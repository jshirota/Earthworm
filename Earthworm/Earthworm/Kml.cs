﻿using System.Xml.Linq;
using ESRI.ArcGIS.Geometry;

namespace Earthworm;

/// <summary>
/// Provides extension methods for converting geometries into KML.
/// </summary>
public static class Kml
{
    private static readonly XNamespace kml = "http://www.opengis.net/kml/2.2";

    #region Private

    private static string ToString(this double[][] coordinates, double z)
    {
        return string.Join(" ", coordinates.Select(c => c[0] + "," + c[1] + "," + z));
    }

    private static XElement ToKmlPoint(this JsonPoint point, double z, params XElement[] extraElements)
    {
        return new XElement(kml + "Point", extraElements,
            new XElement(kml + "coordinates", point.x + "," + point.y + "," + z));
    }

    private static XElement ToKmlMultipoint(this JsonMultipoint multipoint, double z, params XElement[] extraElements)
    {
        return new XElement(kml + "MultiGeometry",
            multipoint.points.Select(p => new JsonPoint { x = p[0], y = p[1] }.ToKmlPoint(z, extraElements)));
    }

    private static XElement ToKmlPolyline(this JsonPolyline polyline, double z, params XElement[] extraElements)
    {
        return new XElement(kml + "MultiGeometry",
            polyline.paths.Select(p =>
                new XElement(kml + "LineString", extraElements,
                    new XElement(kml + "coordinates", p.ToString(z)))));
    }

    private static XElement ToKmlPolygon(this JsonPolygon polygon, double z, params XElement[] extraElements)
    {
        var polygons = new List<XElement>();

        foreach (var ring in polygon.rings)
        {
            var isInnerRing = ring.IsInnerRing();

            if (polygon.rings.Length == 1 || !isInnerRing)
                polygons.Add(new XElement(kml + "Polygon", extraElements));

            if (polygons.Count == 0)
                throw new ArgumentException("The first ring of a polygon must be an outer ring.");

            polygons.Last().Add(new XElement(kml + (isInnerRing ? "innerBoundaryIs" : "outerBoundaryIs"),
                new XElement(kml + "LinearRing",
                    new XElement(kml + "coordinates", ring.ToString(z)))));
        }

        return new XElement(kml + "MultiGeometry", polygons);
    }

    private static XElement ToKml(this IJsonGeometry shape, double z = 0, params XElement[] extraElements)
    {
        return shape switch
        {
            JsonPoint point => point.ToKmlPoint(z, extraElements),
            JsonMultipoint multipoint => multipoint.ToKmlMultipoint(z, extraElements),
            JsonPolyline polyline => polyline.ToKmlPolyline(z, extraElements),
            JsonPolygon polygon => polygon.ToKmlPolygon(z, extraElements),
            _ => throw new ArgumentException("This geometry type is not supported.", nameof(shape))
        };
    }

    #endregion

    /// <summary>
    /// Converts an ArcObjects geometry into KML.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="z">Altitude in meters</param>
    /// <param name="extraElements">Array of extra elements (i.e. altitudeMode).</param>
    /// <returns></returns>
    public static XElement? ToKml(this IGeometry? shape, double z = 0, params XElement[] extraElements)
    {
        if (shape is null)
            return null;

        return (shape.SpatialReference != null ? shape.Project2(4326) : shape).ToJsonGeometry().ToKml(z, extraElements);
    }

    /// <summary>
    /// Converts the style object to KML.
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public static XElement ToKml(this KmlStyle style)
    {
        return new XElement(kml + "Style", new XAttribute("id", style.GetHashCode()),
                   new XElement(kml + "IconStyle",
                       new XElement(kml + "color", style.IconColour),
                       new XElement(kml + "scale", style.IconScale),
                       new XElement(kml + "Icon", style.IconUrl)),
                   new XElement(kml + "LineStyle",
                       new XElement(kml + "color", style.LineColour),
                       new XElement(kml + "width", style.LineWidth)),
                   new XElement(kml + "PolyStyle",
                       new XElement(kml + "color", style.PolygonColour)));
    }

    /// <summary>
    /// Converts the feature to KML.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="name">The name for the placemark.</param>
    /// <param name="z">The altitude in meters.</param>
    /// <param name="geometryElements">Any extra geometry elements (i.e. altitudeMode).</param>        
    /// <param name="placemarkElements">Any extra placemark elements (i.e. styleUrl).</param>
    /// <returns></returns>
    public static XElement ToKml(this IEntity<IGeometry> item, string? name = null, double z = 0, XElement[]? geometryElements = null, params XElement[] placemarkElements)
    {
        return new XElement(kml + "Placemark", new XAttribute("id", item.OID),
                   new XElement(kml + "name", name), placemarkElements ?? Array.Empty<XElement>(),
                   new XElement(kml + "ExtendedData",
                       from f in item.GetFieldNames(true, true)
                       let o = item[f]
                       select new XElement(kml + "Data", new XAttribute("name", f),
                                  new XElement(kml + "value", o is byte[]? "" : (o is DateTime time ? time.ToString("o") : o)))),
                                     item.Shape.ToKml(z, geometryElements ?? Array.Empty<XElement>()));
    }

    /// <summary>
    /// Converts the feature to KML.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="name"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    public static XElement ToKml(this IEntity<IGeometry> item, string? name = null, KmlStyle? style = null)
    {
        return style is null ? item.ToKml(name, 0, null) : item.ToKml(name, 0, null, style.ToKml());
    }

    /// <summary>
    /// Converts the features to KML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="name">The name for the placemark.</param>
    /// <param name="z">The altitude in meters.</param>
    /// <param name="placemarkElements">Any extra placemark elements.</param>
    /// <param name="documentElements">Any extra document elements.</param>
    /// <returns></returns>
    public static XElement ToKml<T>(this IEnumerable<T> items, Func<T, string>? name, Func<T, double>? z, Func<T, XElement[]>? placemarkElements, params XElement[] documentElements) where T : IEntity<IGeometry>
    {
        return new XElement(kml + "kml",
                   new XElement(kml + "Document", documentElements,
                       items.Select(i => i.ToKml(name?.Invoke(i), z is null ? 0 : z(i), null, placemarkElements?.Invoke(i) ?? Array.Empty<XElement>()))));
    }

    /// <summary>
    /// Converts the features to KML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="name">The name for the placemark.</param>
    /// <param name="z">The altitude in meters.</param>
    /// <param name="style">The style for the placemark.</param>
    /// <param name="placemarkElements">Any extra placemark elements.</param>
    /// <param name="documentElements">Any extra document elements.</param>
    /// <returns></returns>
    public static XElement ToKml<T>(this IEnumerable<T> items, Func<T, string>? name = null, Func<T, double>? z = null, Func<T, KmlStyle>? style = null, Func<T, XElement[]>? placemarkElements = null, params XElement[] documentElements) where T : IEntity<IGeometry>
    {
        if (style == null)
            return items.ToKml(name, z, placemarkElements, documentElements);

        var dictionary = items.Distinct().ToDictionary(i => i, style);

        return dictionary.Keys.ToKml(name, z,
            i => new[] { new XElement(kml + "styleUrl", "#" + dictionary[i].GetHashCode()) }.Concat(placemarkElements == null ? Array.Empty<XElement>() : placemarkElements(i)).ToArray(),
            dictionary.Values.Distinct().Select(ToKml).Concat(documentElements ?? Array.Empty<XElement>()).ToArray());
    }
}

/// <summary>
/// Represents the KML style.
/// </summary>
public class KmlStyle
{
    /// <summary>
    /// The url of the icon.
    /// </summary>
    public string IconUrl { get; private set; }

    /// <summary>
    /// The colour of icons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
    /// </summary>
    public string IconColour { get; private set; }

    /// <summary>
    /// The size of icons.
    /// </summary>
    public double IconScale { get; private set; }

    /// <summary>
    /// The colour of lines.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
    /// </summary>
    public string LineColour { get; private set; }

    /// <summary>
    /// The width of lines.  This applies to polygons, too.
    /// </summary>
    public double LineWidth { get; private set; }

    /// <summary>
    /// The colour of polygons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).
    /// </summary>
    public string PolygonColour { get; private set; }

    /// <summary>
    /// Initializes a new instance of the KmlStyle class.
    /// </summary>
    /// <param name="iconUrl">The url of the icon.</param>
    /// <param name="iconColour">The colour of icons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
    /// <param name="iconScale">The size of icons.</param>
    /// <param name="lineColour">The colour of lines.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
    /// <param name="lineWidth">The width of lines.  This applies to polygons, too.</param>
    /// <param name="polygonColour">The colour of polygons.  The format conforms to the KML specification (i.e. 7fff0000 where alpha=0x7f, blue=0xff, green=0x00, and red=0x00).</param>
    public KmlStyle(string? iconUrl = null, string? iconColour = null, double iconScale = 1.1, string? lineColour = null, double lineWidth = 1.2, string? polygonColour = null)
    {
        IconUrl = iconUrl ?? "http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png";
        IconColour = iconColour ?? "ffffffff";
        IconScale = iconScale;
        LineColour = lineColour ?? "ffffffff";
        LineWidth = lineWidth;
        PolygonColour = polygonColour ?? "ffffffff";
    }

    /// <summary>
    /// Overridden to return the value equality.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj is not KmlStyle s)
            return false;

        return s.IconUrl == IconUrl
            && s.IconColour == IconColour
            && s.IconScale == IconScale
            && s.LineColour == LineColour
            && s.LineWidth == LineWidth
            && s.PolygonColour == PolygonColour;
    }

    /// <summary>
    /// Serves as a hash function for equality comparison.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var n = 23;
            var hash = 17;

            hash = hash * n + IconUrl.GetHashCode();
            hash = hash * n + IconColour.GetHashCode();
            hash = hash * n + IconScale.GetHashCode();
            hash = hash * n + LineColour.GetHashCode();
            hash = hash * n + LineWidth.GetHashCode();
            hash = hash * n + PolygonColour.GetHashCode();

            return hash;
        }
    }
}
