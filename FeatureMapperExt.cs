using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for OR mapping.
    /// </summary>
    public static class FeatureMapperExt
    {
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
            foreach (List<string> batch in oids.Distinct().Select(n => n.ToString()).Partition(100))
            {
                string whereClause = string.Format("{0} in ({1})", table.OIDFieldName, batch.Count == 0 ? "-1" : string.Join(",", batch.ToArray()));

                foreach (T item in table.Map<T>(whereClause))
                    yield return item;
            }
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
            IEnumerable<int> oids = Enumerable.Range(0, int.MaxValue)
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
