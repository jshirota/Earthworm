using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Earthworm.Serialization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Earthworm.AO;

namespace Earthworm
{
    /// <summary>
    /// Represents the base class for objects that are OR-mapped using FeatureMapper.
    /// </summary>
    public abstract class MappableFeature : INotifyPropertyChanged
    {
        private static readonly AttributeValueComparer Comparer = new AttributeValueComparer();
        private readonly Type _type;
        private IGeometry _shape;

        internal ITable Table { get; set; }
        internal ConcurrentDictionary<string, object> ChangedProperties { get; set; }
        internal bool IsBeingSetByFeatureMapper { get; set; }

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
            if (IsBeingSetByFeatureMapper)
                return;

            var propertyChanged = PropertyChanged;

            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == "IsDirty" || ChangedProperties.ContainsKey(propertyName))
                return;

            var added = ChangedProperties.TryAdd(propertyName, null);

            if (added && ChangedProperties.Count == 1)
                RaisePropertyChanged(() => IsDirty);
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

        #region Private

        internal IEnumerable<KeyValuePair<string, object>> ToKeyValuePairs(Func<MappedProperty, bool> predicate = null)
        {
            return from p in _type.GetMappedProperties()
                   where predicate == null || predicate(p)
                   select new KeyValuePair<string, object>(p.MappedField.FieldName, p.GetValue(this, false));
        }

        private class AttributeValueComparer : IEqualityComparer<KeyValuePair<string, object>>
        {
            bool IEqualityComparer<KeyValuePair<string, object>>.Equals(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
            {
                //This assumes that the two items have the same key and the same type of object.

                if (x.Value == null && y.Value == null)
                    return true;

                if (x.Value != null && y.Value != null)
                {
                    if (x.Value.Equals(y.Value))
                        return true;

                    if (x.Value.GetType() == typeof(byte[]))
                        return ((byte[])x.Value).SequenceEqual((byte[])y.Value);
                }

                return false;
            }

            int IEqualityComparer<KeyValuePair<string, object>>.GetHashCode(KeyValuePair<string, object> obj)
            {
                return 0;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MappableFeature class.
        /// </summary>
        protected MappableFeature()
        {
            _type = GetType();
            ChangedProperties = new ConcurrentDictionary<string, object>();
            OID = -1;
        }

        /// <summary>
        /// Instantiates a new object of the specified type.  Use this method instead of the constructor to ensure that the mapped properties automatically raise the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>() where T : MappableFeature
        {
            return NotificationProxy.Create<T>();
        }

        /// <summary>
        /// The OID of the item.  If an item is not bound to a feature class (if it is created from scratch), the OID is set to -1.
        /// </summary>
        public int OID { get; internal set; }

        /// <summary>
        /// The geometry of the item.  The data assigned to this property is always cloned (unless overridden to behave differently).
        /// </summary>
        public virtual IGeometry Shape
        {
            get { return _shape; }
            set
            {
                _shape = value.Copy();
                RaisePropertyChanged("Shape");
            }
        }

        /// <summary>
        /// Indicates if the object is bound to an actual row in the underlying table.
        /// </summary>
        public bool IsDataBound
        {
            get { return Table != null; }
        }

        /// <summary>
        /// Indicates if any of the mapped properties has been changed via the property setter.  Mutating reference type properties (i.e. shape, byte array) does not change this property.
        /// </summary>
        public bool IsDirty
        {
            get { return ChangedProperties.Count > 0; }
        }

        private PropertyInfo FindProperty(string fieldName)
        {
            var property = GetType().GetMappedProperties().FirstOrDefault(p => p.MappedField.FieldName == fieldName);

            if (property == null)
                throw new Exception(string.Format("'{0}' does not exist.", fieldName));

            return property.PropertyInfo;
        }

        /// <summary>
        /// Gets or sets a field value based on the field name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get { return FindProperty(fieldName).GetValue(this, null); }
            set { FindProperty(fieldName).SetValue(this, value, null); }
        }

        /// <summary>
        /// Copies all mapped property values from another instance of the same type.  The OID will not change.  Byte arrays will be cloned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void CopyDataFrom<T>(T item) where T : MappableFeature, new()
        {
            foreach (var property in typeof(T).GetMappedProperties())
            {
                var obj = property.GetValue(item, false);

                var array = obj as byte[];

                if (array != null)
                    obj = array.Clone();

                property.SetValue(this, obj, false);
            }

            Shape = item.Shape;
        }

        /// <summary>
        /// Compares all mapped field values and geometries between two objects.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="compareGeometry">If set to true, the geometries are also compared.</param>
        /// <returns></returns>
        public bool ValueEquals(MappableFeature item, bool compareGeometry = true)
        {
            if (item == null)
                return false;

            if (!item.ToKeyValuePairs().SequenceEqual(ToKeyValuePairs(), Comparer))
                return false;

            if (compareGeometry)
            {
                if (item.Shape == Shape)
                    return true;

                if (item.Shape == null)
                    return false;

                return item.Shape.Equals2(Shape);
            }

            return true;
        }

        /// <summary>
        /// Overridden to return the JSON representation of this feature.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToJson(false);
        }

        /// <summary>
        /// Returns the JSON representation of this feature.
        /// </summary>
        /// <param name="includeGeometry"></param>
        /// <returns></returns>
        public string ToString(bool includeGeometry)
        {
            return this.ToJson(includeGeometry);
        }
    }
}
