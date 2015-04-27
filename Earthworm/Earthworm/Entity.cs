using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// The base class for entities that database rows can be mapped to.
    /// </summary>
    public class Entity : IEntity, INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _temporaryStorage = new Dictionary<string, object>();
        internal IGeometry TemporaryShape;

        private readonly Dictionary<string, PropertyInfo> _mappings;

        internal static readonly Func<Type, Dictionary<Mapped, PropertyInfo>> GetMappings =
            Memoization.Memoize<Type, Dictionary<Mapped, PropertyInfo>>(type =>
                type.GetProperties()
                    .Select(p => new { p, a = Attribute.GetCustomAttributes(p).OfType<Mapped>().SingleOrDefault() })
                    .Where(o => o.a != null)
                    .ToDictionary(o => o.a, o => o.p));

        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        protected Entity()
        {
            _mappings = GetMappings(GetType()).ToDictionary(m => m.Key.FieldName, m => m.Value);
        }

        /// <summary>
        /// Instantiates a new object of the specified type.  Use this method instead of the constructor to ensure that the mapped properties automatically raise the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T Create<T>(IRow row = null) where T : Entity
        {
            var item = Proxy.Create<T>();
            item.Row = row;
            return item;
        }

        internal IRow Row;
        internal bool HasShape;

        /// <summary>
        /// The ObjectID field.
        /// </summary>
        public int OID { get { return IsDataBound ? Row.OID : -1; } }

        /// <summary>
        /// Indicates if the object is bound to an actual row in the underlying table.
        /// </summary>
        public bool IsDataBound { get { return Row != null; } }

        private bool _isDirty;

        /// <summary>
        /// Indicates if any of the mapped properties has been changed via the property setter.  Mutating reference type properties (i.e. shape, byte array) does not change this property.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            internal set
            {
                if (_isDirty == value)
                    return;

                _isDirty = value;

                RaisePropertyChanged(() => IsDirty);
            }
        }

        /// <summary>
        /// Returns the underlying database field names.
        /// </summary>
        /// <param name="includeOID"></param>
        /// <param name="includeGlobalID"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFieldNames(bool includeOID, bool includeGlobalID)
        {
            if (!IsDataBound)
                return _mappings.Keys.Concat(_temporaryStorage.Keys).Distinct();

            return Enumerable.Range(0, Row.Fields.FieldCount)
                .Select(i => Row.Fields.Field[i])
                .Where(f => includeOID || f.Type != esriFieldType.esriFieldTypeOID)
                .Where(f => includeGlobalID || f.Type != esriFieldType.esriFieldTypeGlobalID)
                .Where(f => f.Type != esriFieldType.esriFieldTypeGeometry)
                .Where(f => f.Type != esriFieldType.esriFieldTypeRaster)
                .Where(f => f.Type != esriFieldType.esriFieldTypeXML)
                .Select(f => f.Name);
        }

        internal static bool AreEqual(object o1, object o2)
        {
            if (o1 == null)
                return o2 == null;

            return o1.Equals(o2);
        }

        private object GetValue(string fieldName)
        {
            if (IsDataBound)
                return Row.GetValue(fieldName);

            if (_temporaryStorage.ContainsKey(fieldName))
                return _temporaryStorage[fieldName];

            if (!_mappings.ContainsKey(fieldName))
                throw new MissingFieldException(string.Format("Field '{0}' has not been defined.", fieldName));

            var type = _mappings[fieldName].PropertyType;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private void SetValue(string fieldName, object value)
        {
            if (IsDataBound)
            {
                if (AreEqual(Row.GetValue(fieldName), value))
                    return;

                Row.SetValue(fieldName, value);
            }
            else
            {
                if (_temporaryStorage.ContainsKey(fieldName) && AreEqual(_temporaryStorage[fieldName], value))
                    return;

                _temporaryStorage[fieldName] = value;
            }

            if (_mappings.ContainsKey(fieldName))
                RaisePropertyChanged(_mappings[fieldName].Name);

            IsDirty = true;
        }

        /// <summary>
        /// Gets or sets a field value based on the field name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get { return GetValue(fieldName); }
            set { SetValue(fieldName, value); }
        }

        internal void CopyTo(IRowBuffer rowBuffer)
        {
            var row = (IRow)rowBuffer;

            foreach (var fieldName in GetFieldNames(false, false))
            {
                var fieldIndex = AO.GetFieldIndex(row.Table, fieldName);

                if (fieldIndex == -1)
                    throw new MissingFieldException(string.Format("Field '{0}' does not exist in Table '{1}'.", fieldName, ((IDataset)row.Table).Name));

                var field = row.Table.Fields.Field[fieldIndex];

                if (field.Editable)
                    row.SetValue(fieldIndex, this[fieldName]);
            }

            if (HasShape)
            {
                var feature = row as IFeature;

                if (feature != null)
                    feature.Shape = ((dynamic)this).Shape;
            }
        }

        /// <summary>
        /// Inserts this item into a table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int InsertInto(ITable table)
        {
            var cursor = table.Insert(true);

            try
            {
                var rowBuffer = table.CreateRowBuffer();

                CopyTo(rowBuffer);

                return (int)cursor.InsertRow(rowBuffer);
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0) { }
            }
        }

        internal Action<IRow> UpdateAction;

        /// <summary>
        /// Commits changes to the underlying table.
        /// </summary>
        public void Update()
        {
            if (!IsDataBound)
                throw new InvalidOperationException("Cannot invoke Update because the entity is not bound to an actual row in the database.");

            if (UpdateAction == null)
                Row.Store();
            else
                UpdateAction(Row);

            IsDirty = false;
        }

        internal Action DeleteAction;

        /// <summary>
        /// Deletes this item from the underlying table.
        /// </summary>
        public void Delete()
        {
            if (!IsDataBound)
                throw new InvalidOperationException("Cannot invoke Delete because the entity is not bound to an actual row in the database.");

            _temporaryStorage.Clear();

            _temporaryStorage.Add(Row.Table.OIDFieldName, -1);

            foreach (var fieldName in GetFieldNames(false, false))
                _temporaryStorage.Add(fieldName, this[fieldName]);

            if (HasShape)
                TemporaryShape = ((IFeature)Row).Shape;

            if (DeleteAction == null)
                Row.Delete();
            else
                DeleteAction();

            Row = null;

            IsDirty = false;
        }

        /// <summary>
        /// Represents the method that will handle the PropertyChanged event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called from a property setter to notify the framework that a member has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;

            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called from a property setter to notify the framework that a member has changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector"></param>
        public void RaisePropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            var memberExpression = propertySelector.Body as MemberExpression;

            if (memberExpression != null)
                RaisePropertyChanged(memberExpression.Member.Name);
        }
    }

    /// <summary>
    /// The interface for entities that database rows can be mapped to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Entity<T> : Entity, IEntity<T> where T : class, IGeometry
    {
        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        protected Entity()
        {
            HasShape = true;
        }

        /// <summary>
        /// The geometry of this item.
        /// </summary>
        public T Shape
        {
            get { return (IsDataBound ? ((IFeature)Row).Shape : TemporaryShape) as T; }
            set
            {
                if (IsDataBound)
                {
                    var feature = Row as IFeature;

                    if (feature == null)
                        throw new InvalidProgramException("The underlying row has no shape field.");

                    if (AreEqual(feature.Shape, value))
                        return;

                    feature.Shape = value;
                }
                else
                {
                    if (AreEqual(TemporaryShape, value))
                        return;

                    TemporaryShape = value;
                }

                RaisePropertyChanged(() => Shape);

                IsDirty = true;
            }
        }

        /// <summary>
        /// Inserts this item into a feature class.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public int InsertInto(IFeatureClass featureClass)
        {
            return InsertInto((ITable)featureClass);
        }
    }

    /// <summary>
    /// Provides extension methods for tables and feature classes.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Reads database rows as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="filter"></param>
        /// <param name="useUpdateCursor"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, IQueryFilter filter = null, bool useUpdateCursor = false) where T : Entity
        {
            var cursor = useUpdateCursor
                ? table.Update(filter, false)
                : table.Search(filter, false);

            try
            {
                T item = null;

                while (true)
                {
                    var row = cursor.NextRow();

                    if (item != null)
                    {
                        item.UpdateAction = null;
                        item.DeleteAction = null;
                    }

                    if (row == null)
                        yield break;

                    item = Entity.Create<T>(row);

                    if (useUpdateCursor)
                    {
                        item.UpdateAction = cursor.UpdateRow;
                        item.DeleteAction = cursor.DeleteRow;
                    }

                    yield return item;
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0) { }
            }
        }

        /// <summary>
        /// Reads database rows as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="filter"></param>
        /// <param name="useUpdateCursor"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, IQueryFilter filter = null, bool useUpdateCursor = false) where T : Entity
        {
            return ((ITable)featureClass).Map<T>(filter, useUpdateCursor);
        }

        /// <summary>
        /// Finds the row by the OID and returns the data as an object of the specified type.  If no match is found, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static T FindItemByOID<T>(this ITable table, int oid) where T : Entity
        {
            IRow row;

            try { row = table.GetRow(oid); }
            catch { return null; }

            return Entity.Create<T>(row);
        }

        /// <summary>
        /// Finds the row by the OID and returns the data as an object of the specified type.  If no match is found, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static T FindItemByOID<T>(this IFeatureClass featureClass, int oid) where T : Entity
        {
            return ((ITable)featureClass).FindItemByOID<T>(oid);
        }

        /// <summary>
        /// Inserts multiple items into a table using an insert cursor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int[] Insert<T>(this ITable table, IEnumerable<T> items) where T : Entity
        {
            var cursor = table.Insert(true);

            try
            {
                var oids = new List<int>();

                foreach (var item in items)
                {
                    if (oids.Contains(item.OID))
                        break;

                    var rowBuffer = table.CreateRowBuffer();

                    item.CopyTo(rowBuffer);

                    oids.Add((int)cursor.InsertRow(rowBuffer));
                }

                return oids.ToArray();
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0) { }
            }
        }

        /// <summary>
        /// Inserts multiple items into a table using an insert cursor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int[] Insert<T>(this IFeatureClass featureClass, IEnumerable<T> items) where T : Entity
        {
            return ((ITable)featureClass).Insert(items);
        }
    }
}
