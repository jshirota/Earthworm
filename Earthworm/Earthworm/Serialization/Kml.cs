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

        private static XElement ToKmlPoint(this JsonPoint shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            return new XElement(ns + "Point", extraElements,
                new XElement(ns + "coordinates", shape.ToCoordinates(z)));
        }

        private static XElement ToKmlMultipoint(this JsonMultipoint shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            return new XElement(ns + "MultiGeometry",
                shape.points.Select(p => new JsonPoint { x = p[0], y = p[1] }.ToKmlPoint(ns, z, extraElements)));
        }

        private static XElement ToKmlPolyline(this JsonPolyline shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            return new XElement(ns + "MultiGeometry",
                shape.paths.Select(p =>
                    new XElement(ns + "LineString", extraElements,
                        new XElement(ns + "coordinates", p.ToCoordinates(z)))));
        }

        private static XElement ToKmlPolygon(this JsonPolygon shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            var polygons = new List<XElement>();

            foreach (var ring in shape.rings)
            {
                var isInnerRing = IsInnerRing(ring);

                if (!isInnerRing)
                    polygons.Add(new XElement(ns + "Polygon", extraElements));

                polygons.Last().Add(new XElement(ns + (isInnerRing ? "innerBoundaryIs" : "outerBoundaryIs"),
                    new XElement(ns + "LinearRing",
                        new XElement(ns + "coordinates", ring.ToCoordinates(z)))));
            }

            return new XElement(ns + "MultiGeometry", polygons);
        }

        #endregion

        /// <summary>
        /// Converts a serializable geometry into KML.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="ns">XML namespace (i.e. "http://www.opengis.net/kml/2.2").</param>
        /// <param name="z">Altitude in meters</param>
        /// <param name="extraElements">Array of extra elements (i.e. altitudeMode).</param>
        /// <returns></returns>
        public static XElement ToKml(this IJsonGeometry shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            var point = shape as JsonPoint;
            if (point != null)
                return point.ToKmlPoint(ns, z, extraElements);

            var multipoint = shape as JsonMultipoint;
            if (multipoint != null)
                return multipoint.ToKmlMultipoint(ns, z, extraElements);

            var polyline = shape as JsonPolyline;
            if (polyline != null)
                return polyline.ToKmlPolyline(ns, z, extraElements);

            var polygon = shape as JsonPolygon;
            if (polygon != null)
                return polygon.ToKmlPolygon(ns, z, extraElements);

            throw new Exception("This geometry type is not supported.");
        }

        /// <summary>
        /// Converts an ArcObjects geometry into KML.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="ns">XML namespace (i.e. "http://www.opengis.net/kml/2.2").</param>
        /// <param name="z">Altitude in meters</param>
        /// <param name="extraElements">Array of extra elements (i.e. altitudeMode).</param>
        /// <returns></returns>
        public static XElement ToKml(this IGeometry shape, XNamespace ns, double z, params XElement[] extraElements)
        {
            return shape.ToJsonGeometry().ToKml(ns, z, extraElements);
        }

        /// <summary>
        /// Converts an ArcObjects geometry into KML.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="ns">XML namespace (i.e. "http://www.opengis.net/kml/2.2").</param>
        /// <returns></returns>
        public static XElement ToKml(this IGeometry shape, XNamespace ns)
        {
            return shape.ToKml(ns, 0);
        }

        /// <summary>
        /// Converts an ArcObjects geometry into KML.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static XElement ToKml(this IGeometry shape)
        {
            return shape.ToKml("http://www.opengis.net/kml/2.2", 0);
        }

        /// <summary>
        /// Converts the feature to KML.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="z"></param>
        /// <param name="geometryElements"></param>
        /// <param name="placemarkElements"></param>
        /// <returns></returns>
        public static XElement ToKml(this MappableFeature item, string name, double z = 0, XElement[] geometryElements = null, params XElement[] placemarkElements)
        {
            return new XElement(kml + "Placemark",
                       new XElement(kml + "name", name), placemarkElements,
                       new XElement(kml + "ExtendedData",
                           from p in item.GetType().GetMappedProperties()
                           select new XElement(kml + "Data", new XAttribute("name", p.MappedField.FieldName),
                                      new XElement(kml + "value", item[p.MappedField.FieldName]))),
                                          item.Shape.ToKml(kml, z, geometryElements));
        }

        /// <summary>
        /// Converts the features to KML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getName"></param>
        /// <param name="getZ"></param>
        /// <param name="getStyleUrl"></param>
        /// <param name="documentElements"></param>
        /// <returns></returns>
        public static XElement ToKml<T>(this IEnumerable<T> items, Func<T, string> getName = null, Func<T, double> getZ = null, Func<T, string> getStyleUrl = null, params XElement[] documentElements) where T : MappableFeature
        {
            var geometryElements = getZ == null ? null : new[] { new XElement(kml + "extrude", 1), new XElement(kml + "altitudeMode", "relativeToGround") };

            return new XElement(kml + "kml",
                       new XElement(kml + "Document", documentElements,
                           items.Select(f =>
                           {
                               var name = getName == null ? f.OID.ToString() : getName(f);
                               var z = getZ == null ? 0 : getZ(f);
                               var placemarkElements = getStyleUrl == null ? null : new XElement(kml + "styleUrl", getStyleUrl(f));
                               return f.ToKml(name, z, geometryElements, placemarkElements);
                           })));
        }
    }
}
