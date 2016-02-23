using System;
using System.Collections;
using System.Linq;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for converting features to text formats.
    /// </summary>
    public static class Text
    {
        private static string ToDelimitedText(this IEntity item, string delimiter = ",", char? qualifier = '"', Func<IGeometry, object> geometrySelector = null, Func<DateTime, object> dateSelector = null)
        {
            if (string.IsNullOrEmpty(delimiter))
                throw new ArgumentException("The delimiter is required.", nameof(delimiter));

            var q = qualifier.ToString();

            if (q != "" && delimiter.Contains(q))
                throw new ArgumentException("The qualifier is not valid.", nameof(qualifier));

            var fieldNames = item.GetFieldNames(true, true);

            var values = fieldNames.Select(n => item[n]).ToList();

            if (geometrySelector != null)
            {
                var entity = item as IEntity<IGeometry>;

                if (entity != null)
                {
                    var o = geometrySelector(entity.Shape);

                    if (o is string)
                        values.Add(o);
                    else
                        values.AddRange((o as IEnumerable ?? new[] { o }).Cast<object>());
                }
            }

            dateSelector = dateSelector ?? (d => d.ToString("o"));

            return string.Join(delimiter, values.Select(o =>
            {
                if (o is byte[])
                    o = "";

                else if (o is DateTime)
                    o = dateSelector((DateTime)o);

                if (q == "")
                    return o;

                return qualifier + (o ?? "").ToString().Replace(q, q + q) + q;
            }));
        }

        /// <summary>
        /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="delimiter"></param>
        /// <param name="qualifier"></param>
        /// <param name="geometrySelector"></param>
        /// <param name="dateSelector"></param>
        /// <returns></returns>
        public static string ToText<T>(this IEntity<T> item, string delimiter, char? qualifier, Func<T, object> geometrySelector = null, Func<DateTime, object> dateSelector = null) where T : IGeometry
        {
            return item.ToDelimitedText(delimiter, qualifier, geometrySelector == null ? (Func<IGeometry, object>)null : g => geometrySelector((T)g), dateSelector);
        }

        /// <summary>
        /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="geometrySelector"></param>
        /// <param name="dateSelector"></param>
        /// <returns></returns>
        public static string ToText<T>(this IEntity<T> item, Func<T, object> geometrySelector = null, Func<DateTime, object> dateSelector = null) where T : IGeometry
        {
            return item.ToDelimitedText(",", '"', geometrySelector == null ? (Func<IGeometry, object>)null : g => geometrySelector((T)g), dateSelector);
        }

        /// <summary>
        /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="delimiter"></param>
        /// <param name="qualifier"></param>
        /// <param name="dateSelector"></param>
        /// <returns></returns>
        public static string ToText(this IEntity item, string delimiter = ",", char? qualifier = '"', Func<DateTime, object> dateSelector = null)
        {
            return item.ToDelimitedText(delimiter, qualifier, null, dateSelector);
        }
    }
}
