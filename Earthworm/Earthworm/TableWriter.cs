using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for creating geodatabase tables based on the entity type (a.k.a. "code first").
    /// </summary>
    public static class TableWriter
    {
        #region Private

        private static IField GetOIDField(string fieldName)
        {
            IField field = new Field();

            var fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = fieldName;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;

            return field;
        }

        private static IField GetShapeField(esriGeometryType geometryType, ISpatialReference spatialReference)
        {
            var spatialReferenceResolution = (ISpatialReferenceResolution)spatialReference;
            spatialReferenceResolution.ConstructFromHorizon();
            spatialReferenceResolution.SetDefaultXYResolution();

            var spatialReferenceTolerance = (ISpatialReferenceTolerance)spatialReference;
            spatialReferenceTolerance.SetDefaultXYTolerance();

            IGeometryDef geometryDef = new GeometryDef();
            var geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geometryType;
            geometryDefEdit.SpatialReference_2 = spatialReference;

            IField field = new Field();

            var fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = "Shape";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geometryDef;

            return field;
        }

        private static IField CreateField(string name, esriFieldType type, bool isNullable, int? length)
        {
            IField field = new Field();

            var fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = name;
            fieldEdit.Type_2 = type;
            fieldEdit.IsNullable_2 = isNullable;

            if (type == esriFieldType.esriFieldTypeString && length != null)
                fieldEdit.Length_2 = (int)length;

            return field;
        }

        private static IField CreateField(Type propertyType, string fieldName, int? textLength)
        {
            var t = propertyType;

            var name = fieldName;

            if (t == typeof(string))
                return CreateField(name, esriFieldType.esriFieldTypeString, true, textLength);

            if (t == typeof(short))
                return CreateField(name, esriFieldType.esriFieldTypeSmallInteger, false, null);

            if (t == typeof(short?))
                return CreateField(name, esriFieldType.esriFieldTypeSmallInteger, true, null);

            if (t == typeof(int) || t == typeof(long))
                return CreateField(name, esriFieldType.esriFieldTypeInteger, false, null);

            if (t == typeof(int?) || t == typeof(long?))
                return CreateField(name, esriFieldType.esriFieldTypeInteger, true, null);

            if (t == typeof(decimal) || t == typeof(float))
                return CreateField(name, esriFieldType.esriFieldTypeSingle, false, null);

            if (t == typeof(decimal?) || t == typeof(float?))
                return CreateField(name, esriFieldType.esriFieldTypeSingle, true, null);

            if (t == typeof(double))
                return CreateField(name, esriFieldType.esriFieldTypeDouble, false, null);

            if (t == typeof(double?))
                return CreateField(name, esriFieldType.esriFieldTypeDouble, true, null);

            if (t == typeof(DateTime))
                return CreateField(name, esriFieldType.esriFieldTypeDate, false, null);

            if (t == typeof(DateTime?))
                return CreateField(name, esriFieldType.esriFieldTypeDate, true, null);

            if (t == typeof(byte[]))
                return CreateField(name, esriFieldType.esriFieldTypeBlob, true, null);

            if (t == typeof(Guid))
                return CreateField(name, esriFieldType.esriFieldTypeGUID, false, null);

            if (t == typeof(Guid?))
                return CreateField(name, esriFieldType.esriFieldTypeGUID, true, null);

            throw new ArgumentException(string.Format("This property type '{0}' is not supported.", propertyType.Name), "propertyType");
        }

        private static ITable CreateTable(object container, string name, string oidField, bool isSpatial, esriGeometryType geometryType, ISpatialReference spatialReference, List<IField> customFields)
        {
            var i = (isSpatial ? 2 : 1);

            IFields fields = new Fields();

            var fieldsEdit = (IFieldsEdit)fields;
            fieldsEdit.FieldCount_2 = i + customFields.Count;
            fieldsEdit.set_Field(0, GetOIDField(oidField));

            if (isSpatial)
                fieldsEdit.set_Field(1, GetShapeField(geometryType, spatialReference));

            foreach (var field in customFields)
                fieldsEdit.set_Field(i++, field);

            IObjectClassDescription ocDesc = new FeatureClassDescription();

            var fws = container as IFeatureWorkspace;
            if (fws != null)
                return isSpatial
                    ? fws.CreateFeatureClass(name, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "Shape", "") as ITable
                    : fws.CreateTable(name, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, "");

            var fds = container as IFeatureDataset;
            if (fds != null)
                return fds.CreateFeatureClass(name, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "Shape", "") as ITable;

            return null;
        }

        private static ITable CreateTable<T>(object container, string name, bool isSpatial, esriGeometryType geometryType, ISpatialReference spatialReference) where T : IEntity
        {
            var fields = Entity.GetMappings(typeof(T)).Select(m => CreateField(m.Value.PropertyType, m.Key.FieldName, m.Key.Length)).ToList();

            var fieldNames = fields.Select(f => f.Name.ToUpper()).ToList();

            var defaultName = "OBJECTID";

            var oidField = defaultName;
            var i = 0;

            while (fieldNames.Contains(oidField))
                oidField = defaultName + ++i;

            return CreateTable(container, name, oidField, isSpatial, geometryType, spatialReference, fields);
        }

        #endregion

        /// <summary>
        /// Creates a new table based on the entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureWorkspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITable CreateTable<T>(this IFeatureWorkspace featureWorkspace, string name) where T : IEntity
        {
            return CreateTable<T>(featureWorkspace, name, false, esriGeometryType.esriGeometryNull, null);
        }

        /// <summary>
        /// Creates a new feature class based on the entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureWorkspace"></param>
        /// <param name="name"></param>
        /// <param name="geometryType"></param>
        /// <param name="spatialReference"></param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass<T>(this IFeatureWorkspace featureWorkspace, string name, esriGeometryType geometryType, ISpatialReference spatialReference) where T : IEntity<IGeometry>
        {
            return CreateTable<T>(featureWorkspace, name, true, geometryType, spatialReference) as IFeatureClass;
        }

        /// <summary>
        /// Creates a new feature class based on the entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureWorkspace"></param>
        /// <param name="name"></param>
        /// <param name="geometryType"></param>
        /// <param name="wkid"></param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass<T>(this IFeatureWorkspace featureWorkspace, string name, esriGeometryType geometryType, int wkid) where T : IEntity<IGeometry>
        {
            return CreateTable<T>(featureWorkspace, name, true, geometryType, AO.GetSpatialReference(wkid)) as IFeatureClass;
        }

        /// <summary>
        /// Creates a new feature class based on the entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureDataset"></param>
        /// <param name="name"></param>
        /// <param name="geometryType"></param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass<T>(this IFeatureDataset featureDataset, string name, esriGeometryType geometryType) where T : IEntity<IGeometry>
        {
            return CreateTable<T>(featureDataset, name, true, geometryType, ((IGeoDataset)featureDataset).SpatialReference) as IFeatureClass;
        }
    }
}
