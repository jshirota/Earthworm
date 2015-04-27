using System;
using System.Linq;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for converting features to text formats.
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="delimiter"></param>
        /// <param name="qualifier"></param>
        /// <param name="geometrySelector"></param>
        /// <returns></returns>
        public static string ToText(this IEntity item, string delimiter = ",", char? qualifier = '"', Func<IGeometry, object> geometrySelector = null)
        {
            if (string.IsNullOrEmpty(delimiter))
                throw new ArgumentException("The delimiter is required.", "delimiter");

            var q = qualifier.ToString();

            if (q != "" && delimiter.Contains(q))
                throw new ArgumentException("The qualifier is not valid.", "delimiter");

            var fieldNames = item.GetFieldNames(true, true, false);

            return string.Join(delimiter, fieldNames.Select(n => item[n]).Where(o => !(o is byte[])).Select(o =>
            {
                if (o is DateTime)
                    o = ((DateTime)o).ToString("o");

                if (geometrySelector != null && o is IGeometry)
                    o = geometrySelector((IGeometry)o);

                if (q == "")
                    return o;

                return qualifier + (o ?? "").ToString().Replace(q, q + q) + q;
            }));
        }
    }
}
