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
        /// <param name="dateFormatter"></param>
        /// <param name="geometrySelector"></param>
        /// <returns></returns>
        public static string ToDelimitedText(this MappableFeature item, string delimiter = ",", char? qualifier = '"', Func<DateTime, string> dateFormatter = null, Func<IGeometry, object> geometrySelector = null)
        {
            if (string.IsNullOrEmpty(delimiter))
                throw new Exception("The delimiter is required.");

            var q = qualifier.ToString();

            if (q != "" && delimiter.Contains(q))
                throw new Exception("The qualifier is not valid.");

            var values = item.ToKeyValuePairs().Select(o => o.Value).ToList();

            if (geometrySelector != null)
                values.Add(item.Shape);

            return string.Join(delimiter, values.Select(o =>
            {
                if (dateFormatter != null && o is DateTime)
                    o = dateFormatter((DateTime)o);

                if (geometrySelector != null && o is IGeometry)
                    o = geometrySelector((IGeometry)o);

                if (q == "")
                    return o;

                return qualifier + (o ?? "").ToString().Replace(q, q + q) + q;
            }));
        }
    }
}
