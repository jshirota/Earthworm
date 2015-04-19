using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;

namespace Earthworm
{
    internal class FeatureMapper<T> where T : MappableFeature, new()
    {
        private readonly ITable _table;
        private readonly bool _isSpatial;
        private readonly Dictionary<int, MappedProperty> _mapping = new Dictionary<int, MappedProperty>();
        private readonly List<int> _keyFieldIndexes = new List<int>();
        private readonly List<int> _readOnlyFieldIndexes = new List<int>();

        #region Private

        private T Read(IRow row)
        {
            var item = NotificationProxy.Create<T>();

            item.Table = _table;
            item.OID = row.OID;
            item.IsBeingSetByFeatureMapper = true;
            item.Shape = _isSpatial ? ((IFeature)row).Shape : null;

            foreach (var fieldIndex in _mapping.Keys)
            {
                var mappedProperty = _mapping[fieldIndex];

                var value = row.GetValue(fieldIndex);
                mappedProperty.SetValue(item, value, true);
            }

            item.IsBeingSetByFeatureMapper = false;

            return item;
        }

        private T Write(T item, IRow row, bool isUpdate)
        {
            foreach (var fieldIndex in _mapping.Keys)
            {
                if (isUpdate && _keyFieldIndexes.Contains(fieldIndex))
                    continue;

                if (_readOnlyFieldIndexes.Contains(fieldIndex))
                    continue;

                var mappedProperty = _mapping[fieldIndex];

                if (isUpdate && !item.ChangedProperties.ContainsKey(mappedProperty.PropertyInfo.Name))
                    continue;

                var value = mappedProperty.GetValue(item, true);
                row.SetValue(fieldIndex, value);
            }

            if (_isSpatial && !(isUpdate && !item.ChangedProperties.ContainsKey("Shape")))
                ((IFeature)row).Shape = item.Shape;

            row.Store();

            return Read(_table.GetRow(row.OID));
        }

        #endregion

        public FeatureMapper(ITable table)
        {
            if (!table.HasOID)
                throw new Exception(string.Format("'{0}' does not have an OID field.", ((IDataset)table).Name));

            _table = table;
            _isSpatial = table is IFeatureClass;

            var keyFields = new List<string>();

            var relationshipClass = table as IRelationshipClass;

            if (relationshipClass != null)
            {
                keyFields.Add(relationshipClass.OriginForeignKey);
                keyFields.Add(relationshipClass.DestinationForeignKey);
            }

            foreach (var mappedProperty in typeof(T).GetMappedProperties())
            {
                var fieldName = mappedProperty.MappedField.FieldName;

                var fieldIndex = table.FindField(fieldName);

                if (fieldIndex == -1)
                    throw new Exception(string.Format("'{0}' does not exist in '{1}'.", fieldName, ((IDataset)table).Name));

                if (keyFields.Contains(fieldName))
                    _keyFieldIndexes.Add(fieldIndex);

                if (!table.Fields.Field[fieldIndex].Editable)
                    _readOnlyFieldIndexes.Add(fieldIndex);

                _mapping.Add(fieldIndex, mappedProperty);
            }
        }

        public T SelectItem(int oid)
        {
            IRow row;

            try { row = _table.GetRow(oid); }
            catch { return null; }

            return Read(row);
        }

        public IEnumerable<T> SelectItems(IQueryFilter filter)
        {
            return _table.ReadRows(filter).Select(Read);
        }

        public T Insert(T item)
        {
            return Write(item, _table.CreateRow(), false);
        }

        public void Update(T item)
        {
            if (!item.IsDataBound)
                throw new Exception("This item cannot be updated because it is not bound to a table.");

            var item2 = Write(item, _table.GetRow(item.OID), true);

            item.IsBeingSetByFeatureMapper = true;
            item.CopyDataFrom(item2);
            item.IsBeingSetByFeatureMapper = false;
            item.ChangedProperties.Clear();
        }

        public void Delete(T item)
        {
            if (!item.IsDataBound)
                throw new Exception("This item cannot be deleted because it is not bound to a table.");

            _table.GetRow(item.OID).Delete();

            item.Table = null;
            item.OID = -1;
            item.ChangedProperties.Clear();
        }
    }

    /// <summary>
    /// Provides extension methods that abstracts database operations.
    /// </summary>
    public static class FeatureMapper
    {
        #region Private

        private static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> items, int size)
        {
            var list = new List<T>();

            foreach (var item in items)
            {
                list.Add(item);

                if (list.Count == size)
                {
                    yield return list;
                    list = new List<T>();
                }
            }

            if (list.Count > 0)
                yield return list;
        }

        #endregion

        /// <summary>
        /// Finds a row by the OID and returns the data as an object of the specified type.  If no match is found, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static T FindItemByOID<T>(this ITable table, int oid) where T : MappableFeature, new()
        {
            return new FeatureMapper<T>(table).SelectItem(oid);
        }

        /// <summary>
        /// Finds a feature by the OID and returns the data as an object of the specified type.  If no match is found, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static T FindItemByOID<T>(this IFeatureClass featureClass, int oid) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).FindItemByOID<T>(oid);
        }

        /// <summary>
        /// Reads all rows meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="filter">The query filter.</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, IQueryFilter filter) where T : MappableFeature, new()
        {
            return new FeatureMapper<T>(table).SelectItems(filter);
        }

        /// <summary>
        /// Reads all rows as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table) where T : MappableFeature, new()
        {
            return table.Map<T>(null as IQueryFilter);
        }

        /// <summary>
        /// Reads all rows meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="whereClause">SQL where clause.</param>
        /// <param name="postfixClause">A clause that will be appended to the end of the where clause (i.e. ORDER BY).</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, string whereClause, string postfixClause) where T : MappableFeature, new()
        {
            IQueryFilter filter = new QueryFilter { WhereClause = whereClause };

            if (!string.IsNullOrEmpty(postfixClause))
                ((IQueryFilterDefinition)filter).PostfixClause = postfixClause;

            return new FeatureMapper<T>(table).SelectItems(filter);
        }

        /// <summary>
        /// Reads all rows meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="whereClause">SQL where clause.</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, string whereClause) where T : MappableFeature, new()
        {
            return table.Map<T>(whereClause, null);
        }

        /// <summary>
        /// Reads all rows whose OIDs are within the specified collection and returns them as a (lazily-evaluated) sequence of objects of the specified type.  The returned collection is not sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="oids"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, IEnumerable<int> oids) where T : MappableFeature, new()
        {
            return oids
                .Distinct()
                .Partition(100)
                .Select(batch => batch.ToArray())
                .Select(array => string.Format("{0} in ({1})", table.OIDFieldName, array.Length == 0 ? "-1" : string.Join(",", array)))
                .SelectMany(s => table.Map<T>(s)); //Do not convert to a method group.
        }

        /// <summary>
        /// Reads all rows whose OIDs are within the specified collection and returns them as a (lazily-evaluated) sequence of objects of the specified type.  The returned collection is not sorted.  This method does not "reset" the input IEnumIDs object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="enumIDs"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this ITable table, IEnumIDs enumIDs) where T : MappableFeature, new()
        {
            var oids = Enumerable.Range(0, int.MaxValue)
                .Select(n => enumIDs.Next())
                .TakeWhile(n => n != -1);

            return table.Map<T>(oids);
        }

        /// <summary>
        /// Reads all features meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="filter">The query filter.</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, IQueryFilter filter) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>(filter);
        }

        /// <summary>
        /// Reads all features as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>();
        }

        /// <summary>
        /// Reads all features meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="whereClause">SQL where clause.</param>
        /// <param name="postfixClause">A clause that will be appended to the end of the where clause (i.e. ORDER BY).</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, string whereClause, string postfixClause) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>(whereClause, postfixClause);
        }

        /// <summary>
        /// Reads all features meeting the filter criteria as a (lazily-evaluated) sequence of objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="whereClause">SQL where clause.</param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, string whereClause) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>(whereClause);
        }

        /// <summary>
        /// Reads all features whose OIDs are within the specified collection and returns them as a (lazily-evaluated) sequence of objects of the specified type.  The returned collection is not sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="oids"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, IEnumerable<int> oids) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>(oids);
        }

        /// <summary>
        /// Reads all features whose OIDs are within the specified collection and returns them as a (lazily-evaluated) sequence of objects of the specified type.  The returned collection is not sorted.  This method does not "reset" the input IEnumIDs object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="enumIDs"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IFeatureClass featureClass, IEnumIDs enumIDs) where T : MappableFeature, new()
        {
            return ((ITable)featureClass).Map<T>(enumIDs);
        }

        /// <summary>
        /// Inserts this item into a table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item to insert.</param>
        /// <param name="table">The target table.</param>
        /// <returns>The inserted item (with a newly assigned OID).</returns>
        public static T InsertInto<T>(this T item, ITable table) where T : MappableFeature, new()
        {
            return new FeatureMapper<T>(table).Insert(item);
        }

        /// <summary>
        /// Inserts this item into a feature class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item to insert.</param>
        /// <param name="featureClass">The target feature class.</param>
        /// <returns>The inserted item (with a newly assigned OID).</returns>
        public static T InsertInto<T>(this T item, IFeatureClass featureClass) where T : MappableFeature, new()
        {
            return item.InsertInto((ITable)featureClass);
        }

        /// <summary>
        /// Updates this item in the underlying table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item to update.  The item must be bound to a table and must have a valid OID.</param>
        public static void Update<T>(this T item) where T : MappableFeature, new()
        {
            new FeatureMapper<T>(item.Table).Update(item);
        }

        /// <summary>
        /// Deletes this item from the underlying table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item to delete.  The item must be bound to a table and must have a valid OID.</param>
        public static void Delete<T>(this T item) where T : MappableFeature, new()
        {
            new FeatureMapper<T>(item.Table).Delete(item);
        }
    }
}
