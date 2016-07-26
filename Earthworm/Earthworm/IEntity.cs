using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// The interface for entities that database rows can be mapped to.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The ObjectID field.
        /// </summary>
        int OID { get; }

        /// <summary>
        /// Indicates if the object is bound to an actual row in the underlying table.
        /// </summary>
        bool IsDataBound { get; }

        /// <summary>
        /// Indicates if any of the mapped properties has been changed via the property setter.  Mutating reference type properties (i.e. shape, byte array) does not change this property.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Returns the underlying database field names.
        /// </summary>
        /// <param name="includeOID"></param>
        /// <param name="includeGlobalID"></param>
        /// <returns></returns>
        IEnumerable<string> GetFieldNames(bool includeOID, bool includeGlobalID);

        /// <summary>
        /// Gets or sets a field value based on the field name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        object this[string fieldName] { get; set; }

        /// <summary>
        /// Inserts this item into a table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fetchRow"></param>
        /// <returns></returns>
        int InsertInto(ITable table, bool fetchRow = false);

        /// <summary>
        /// Commits changes to the underlying table.
        /// </summary>
        void Update();

        /// <summary>
        /// Deletes this item from the underlying table.
        /// </summary>
        void Delete();
    }

    /// <summary>
    /// The interface for entities that database rows can be mapped to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<out T> : IEntity where T : IGeometry
    {
        /// <summary>
        /// The geometry of this item.
        /// </summary>
        T Shape { get; }

        /// <summary>
        /// Inserts this item into a feature class.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        int InsertInto(IFeatureClass featureClass);
    }
}
