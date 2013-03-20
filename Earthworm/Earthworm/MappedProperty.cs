using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;

namespace Earthworm
{
    internal class MappedProperty
    {
        private Func<object, object> _convertFromEsriType;
        private Func<object, object> _convertToEsriType;

        public PropertyInfo PropertyInfo { get; private set; }
        public Type PropertyType { get; private set; }
        public MappedField MappedField { get; private set; }

        #region Private

        private void SetBlobConversions()
        {
            _convertFromEsriType = value =>
            {
                IMemoryBlobStreamVariant ms = (IMemoryBlobStreamVariant)value;
                ms.ExportToVariant(out value);
                return value;
            };

            _convertToEsriType = value =>
            {
                IMemoryBlobStreamVariant ms = (IMemoryBlobStreamVariant)new MemoryBlobStream();
                ms.ImportFromVariant(value);
                return ms;
            };
        }

        private void SetGuidConversions()
        {
            _convertFromEsriType = value => new Guid((string)value);

            _convertToEsriType = value => ((Guid)value).ToString("B").ToUpper();
        }

        private void SetDefaultConversions(Type t)
        {
            _convertFromEsriType = value =>
            {
                try { value = Convert.ChangeType(value, t); }
                catch { throw new Exception(string.Format("Error reading '{0}'.  Cannot convert '{1}' to '{2}'.", MappedField.FieldName, value, t.FullName)); }
                return value;
            };

            _convertToEsriType = value => value;
        }

        #endregion

        public MappedProperty(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            PropertyType = propertyInfo.PropertyType;
            MappedField = Attribute.GetCustomAttributes(propertyInfo).OfType<MappedField>().SingleOrDefault();

            Type t = (PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? new NullableConverter(PropertyType).UnderlyingType
                : PropertyType;

            if (t == typeof(byte[]))
                SetBlobConversions();
            else if (t == typeof(Guid))
                SetGuidConversions();
            else
                SetDefaultConversions(t);
        }

        public object GetValue(object obj, bool convertToEsriType)
        {
            object value = PropertyInfo.GetValue(obj, null);

            if (convertToEsriType && value != null)
                value = _convertToEsriType(value);

            return value;
        }

        public void SetValue(object obj, object value, bool convertFromEsriType)
        {
            if (convertFromEsriType && value != null)
                value = _convertFromEsriType(value);

            PropertyInfo.SetValue(obj, value, null);
        }
    }
}
