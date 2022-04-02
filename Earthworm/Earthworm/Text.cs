using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace Earthworm;

/// <summary>
/// Provides extension methods for converting features to text formats.
/// </summary>
public static class Text
{
    private static string ToDelimitedText(this IEntity item, string delimiter = ",", char? qualifier = '"', Func<IGeometry?, object>? geometrySelector = null, Func<DateTime, object>? dateSelector = null)
    {
        if (string.IsNullOrEmpty(delimiter))
            throw new ArgumentException("The delimiter is required.", nameof(delimiter));

        var q = qualifier.ToString()!;

        if (q != "" && delimiter.Contains(q))
            throw new ArgumentException("The qualifier is not valid.", nameof(qualifier));

        var fieldNames = item.GetFieldNames(true, true);

        var values = fieldNames.Select(n => item[n]).ToList();

        if (geometrySelector is not null && item is IEntity<IGeometry> entity)
        {
            var o = geometrySelector(entity.Shape);
            values.AddRange(o is not string && o is IEnumerable seq ? seq.Cast<object>() : new[] { o });
        }

        return string.Join(delimiter, values.Select(o =>
        {
            if (o is byte[])
                o = "";

            else if (o is DateTime date)
                o = dateSelector is null ? date.ToString("o") : dateSelector(date);

            if (q == "")
                return o;

            return qualifier + (o ?? "").ToString()!.Replace(q, q + q) + q;
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
    public static string ToText<T>(this IEntity<T> item, string delimiter, char? qualifier, Func<T?, object>? geometrySelector = null, Func<DateTime, object>? dateSelector = null)
        where T : class, IGeometry
    {
        return item.ToDelimitedText(delimiter, qualifier, geometrySelector == null ? null : g => geometrySelector(g as T), dateSelector);
    }

    /// <summary>
    /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="geometrySelector"></param>
    /// <param name="dateSelector"></param>
    /// <returns></returns>
    public static string ToText<T>(this IEntity<T> item, Func<T?, object>? geometrySelector = null, Func<DateTime, object>? dateSelector = null)
        where T : class, IGeometry
    {
        return item.ToDelimitedText(",", '"', geometrySelector == null ? null : g => geometrySelector(g as T), dateSelector);
    }

    /// <summary>
    /// Converts the feature attributes to delimiter-separated values (i.e. CSV).
    /// </summary>
    /// <param name="item"></param>
    /// <param name="delimiter"></param>
    /// <param name="qualifier"></param>
    /// <param name="dateSelector"></param>
    /// <returns></returns>
    public static string ToText(this IEntity item, string delimiter = ",", char? qualifier = '"', Func<DateTime, object>? dateSelector = null)
    {
        return item.ToDelimitedText(delimiter, qualifier, null, dateSelector);
    }
}
