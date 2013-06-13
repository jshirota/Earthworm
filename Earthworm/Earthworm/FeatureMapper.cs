using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using Earthworm.AO;

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
            T item = NotificationProxy.Create<T>();

            item.Table = _table;
            item.OID = row.OID;
            item.IsBeingSetByFeatureMapper = true;
            item.Shape = _isSpatial ? ((IFeature)row).Shape : null;

            foreach (int fieldIndex in _mapping.Keys)
            {
                MappedProperty mappedProperty = _mapping[fieldIndex];

                object value = row.GetValue(fieldIndex);
                mappedProperty.SetValue(item, value, true);
            }

            item.IsBeingSetByFeatureMapper = false;

            return item;
        }

        private T Write(T item, IRow row, bool isUpdate)
        {
            foreach (int fieldIndex in _mapping.Keys)
            {
                if (isUpdate && _keyFieldIndexes.Contains(fieldIndex))
                    continue;

                if (_readOnlyFieldIndexes.Contains(fieldIndex))
                    continue;

                MappedProperty mappedProperty = _mapping[fieldIndex];

                if (isUpdate && !item.ChangedProperties.ContainsKey(mappedProperty.PropertyInfo.Name))
                    continue;

                object value = mappedProperty.GetValue(item, true);
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

            List<string> keyFields = new List<string>();

            IRelationshipClass relationshipClass = table as IRelationshipClass;

            if (relationshipClass != null)
            {
                keyFields.Add(relationshipClass.OriginForeignKey);
                keyFields.Add(relationshipClass.DestinationForeignKey);
            }

            foreach (MappedProperty mappedProperty in typeof(T).GetMappedProperties())
            {
                string fieldName = mappedProperty.MappedField.FieldName;

                int fieldIndex = table.FindField(fieldName);

                if (fieldIndex == -1)
                    throw new Exception(string.Format("'{0}' does not exist in '{1}'.", fieldName, ((IDataset)table).Name));

                if (keyFields.Contains(fieldName))
                    _keyFieldIndexes.Add(fieldIndex);

                if (!table.Fields.get_Field(fieldIndex).Editable)
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

            T item2 = Write(item, _table.GetRow(item.OID), true);

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
}
