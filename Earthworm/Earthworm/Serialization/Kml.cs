using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.Serialization
{
    /// <summary>
    /// Provides extension methods for converting geometries into KML.
    /// </summary>
    public static class Kml
    {
        private static readonly XNamespace kml = "http://www.opengis.net/kml/2.2";

        #region Private

        private static string ToCoordinates(this JsonPoint shape, double z)
        {
            return shape.x + "," + shape.y + "," + z;
        }

        private static string ToCoordinates(this double[] coordinates, double z)
        {
            return coordinates[0] + "," + coordinates[1] + "," + z;
        }

        private static string ToCoordinates(this double[][] coordinates, double z)
        {
            return string.Join(" ", coordinates.Select(c => c.ToCoordinates(z)));
        }

        private static bool IsInnerRing(double[][] ring)
        {
            return Enumerable.Range(0, ring.Length - 1)
                .Sum(i => ring[i][0] * ring[i + 1][1] - ring[i + 1][0] * ring[i][1]) > 0;
        }

        private static XElement ToKmlPoint(this JsonPoint shape, double z, params XElement[] extraElements)
        {
            return new XElement(kml + "Point", extraElements,
                new XElement(kml + "coordinates", shape.ToCoordinates(z)));
        }

        private static XElement ToKmlMultipoint(this JsonMultipoint shape, double z, params XElement[] extraElements)
        {
            return new XElement(kml + "MultiGeometry",
                shape.points.Select(p => new JsonPoint { x = p[0], y = p[1] }.ToKmlPoint(z, extraElements)));
        }

        private static XElement ToKmlPolyline(this JsonPolyline shape, double z, params XElement[] extraElements)
        {
            return new XElement(kml + "MultiGeometry",
                shape.paths.Select(p =>
                    new XElement(kml + "LineString", extraElements,
                        new XElement(kml + "coordinates", p.ToCoordinates(z)))));
        }

        private static XElement ToKmlPolygon(this JsonPolygon shape, double z, params XElement[] extraElements)
        {
            var polygons = new List<XElement>();

            foreach (var ring in shape.rings)
            {
                var isInnerRing = IsInnerRing(ring);

                if (shape.rings.Length == 1 || !isInnerRing)
                    polygons.Add(new XElement(kml + "Polygon", extraElements));

                polygons.Last().Add(new XElement(kml + (isInnerRing ? "innerBoundaryIs" : "outerBoundaryIs"),
                    new XElement(kml + "LinearRing",
                        new XElement(kml + "coordinates", ring.ToCoordinates(z)))));
            }

            return new XElement(kml + "MultiGeometry", polygons);
        }

        private static XElement ToKml(this IJsonGeometry shape, double z = 0, params XElement[] extraElements)
        {
            var point = shape as JsonPoint;
            if (point != null)
                return point.ToKmlPoint(z, extraElements);

            var multipoint = shape as JsonMultipoint;
            if (multipoint != null)
                return multipoint.ToKmlMultipoint(z, extraElements);

            var polyline = shape as JsonPolyline;
            if (polyline != null)
                return polyline.ToKmlPolyline(z, extraElements);

            var polygon = shape as JsonPolygon;
            if (polygon != null)
                return polygon.ToKmlPolygon(z, extraElements);

            throw new Exception("This geometry type is not supported.");
        }

        #endregion

        /// <summary>
        /// Converts an ArcObjects geometry into KML.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="z">Altitude in meters</param>
        /// <param name="extraElements">Array of extra elements (i.e. altitudeMode).</param>
        /// <returns></returns>
        public static XElement ToKml(this IGeometry shape, double z = 0, params XElement[] extraElements)
        {
            return shape.ToJsonGeometry().ToKml(z, extraElements);
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
        public static XElement ToKml(this MappableFeature item, string name = null, double? z = null, XElement[] geometryElements = null, params XElement[] placemarkElements)
        {
            return new XElement(kml + "Placemark", new XAttribute("id", item.OID),
                       new XElement(kml + "name", name), placemarkElements,
                       new XElement(kml + "ExtendedData",
                           from p in item.GetType().GetMappedProperties()
                           select new XElement(kml + "Data", new XAttribute("name", p.MappedField.FieldName),
                                      new XElement(kml + "value", item[p.MappedField.FieldName]))),
                                          item.Shape.ToKml(z ?? 0, geometryElements));
        }

        /// <summary>
        /// Converts the feature to KML.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static XElement ToKml(this MappableFeature item, string name = null, KmlStyle style = null)
        {
            return item.ToKml(name, 0, null, style == null ? null : style.ToKml());
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
        public static XElement ToKml<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, double?> z, Func<T, XElement[]> placemarkElements, params XElement[] documentElements) where T : MappableFeature
        {
            return new XElement(kml + "kml",
                       new XElement(kml + "Document", documentElements,
                           items.Select(i => i.ToKml(name == null ? null : name(i), z == null ? null : z(i), null, placemarkElements == null ? null : placemarkElements(i)))));
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
        public static XElement ToKml<T>(this IEnumerable<T> items, Func<T, string> name = null, Func<T, double?> z = null, Func<T, KmlStyle> style = null, Func<T, XElement[]> placemarkElements = null, params XElement[] documentElements) where T : MappableFeature
        {
            if (style == null)
                return items.ToKml(name, z, placemarkElements, documentElements);

            var dictionary = items.Distinct().ToDictionary(i => i, style);

            return dictionary.Keys.ToKml(name, z,
                i => new[] { new XElement(kml + "styleUrl", "#" + dictionary[i].GetHashCode()) }.Concat(placemarkElements == null ? new XElement[] { } : placemarkElements(i)).ToArray(),
                dictionary.Values.Distinct().Select(ToKml).Concat(documentElements ?? new XElement[] { }).ToArray());
        }
    }
}
