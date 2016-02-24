using System;
using System.Linq;
using System.Web.Script.Serialization;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for converting geometries into JSON strings.
    /// </summary>
    public static class Json
    {
        #region Private

        private static T Deserialize<T>(this string json)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Deserialize<T>(json);
        }

        private static string Serialize(this object obj)
        {
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Serialize(obj);
        }

        private static double[][] ToArray(this IPointCollection4 pointCollection)
        {
            return Enumerable.Range(0, pointCollection.PointCount).Select(i =>
            {
                var p = pointCollection.Point[i];
                return new[] { p.X, p.Y };
            }).ToArray();
        }

        private static double[][][] ToArray(this IGeometryCollection geometryCollection)
        {
            return Enumerable.Range(0, geometryCollection.GeometryCount)
                .Select(i => ((IPointCollection4)geometryCollection.Geometry[i]).ToArray())
                .ToArray();
        }

        private static JsonPoint ToPoint(this IPoint shape)
        {
            return new JsonPoint { x = shape.X, y = shape.Y };
        }

        private static JsonMultipoint ToMultipoint(this IMultipoint shape)
        {
            return new JsonMultipoint { points = ((IPointCollection4)shape).ToArray() };
        }

        private static JsonPolyline ToPolyline(this IPolyline shape)
        {
            return new JsonPolyline { paths = ((IGeometryCollection)shape).ToArray() };
        }

        private static JsonPolygon ToPolygon(this IPolygon shape)
        {
            return new JsonPolygon { rings = ((IGeometryCollection)shape).ToArray() };
        }

        internal static IJsonGeometry ToJsonGeometry(this IGeometry shape)
        {
            var point = shape as IPoint;
            if (point != null)
                return point.ToPoint();

            var multipoint = shape as IMultipoint;
            if (multipoint != null)
                return multipoint.ToMultipoint();

            var polyline = shape as IPolyline;
            if (polyline != null)
                return polyline.ToPolyline();

            var polygon = shape as IPolygon;
            if (polygon != null)
                return polygon.ToPolygon();

            throw new ArgumentException("This geometry type is not supported.", "shape");
        }

        #endregion

        /// <summary>
        /// Returns the JSON representation of this shape.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static string ToJson(this IGeometry shape)
        {
            return shape.ToJsonGeometry().Serialize();
        }

        /// <summary>
        /// Converts a JSON string into an Esri IPoint object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IPoint ToPoint(string json)
        {
            var shape = json.Deserialize<JsonPoint>();
            return new PointClass { X = shape.x, Y = shape.y };
        }

        /// <summary>
        /// Converts a JSON string into an Esri IMultipoint object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IMultipoint ToMultipoint(string json)
        {
            var shape = json.Deserialize<JsonMultipoint>();
            return new MultipointClass().Load(shape.points);
        }

        /// <summary>
        /// Converts a JSON string into an Esri IPolyline object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IPolyline ToPolyline(string json)
        {
            var shape = json.Deserialize<JsonPolyline>();
            var polyline = new PolylineClass();

            foreach (var path in shape.paths)
                polyline.AddGeometry(new PathClass().Load(path));

            return polyline;
        }

        /// <summary>
        /// Converts a JSON string into an Esri IPolygon object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IPolygon ToPolygon(string json)
        {
            var shape = json.Deserialize<JsonPolygon>();
            var polygon = new PolygonClass();

            foreach (var ring in shape.rings)
                polygon.AddGeometry(new RingClass().Load(ring));

            return polygon;
        }
    }

    #region Types

    internal interface IJsonGeometry
    {
    }

    internal class JsonPoint : IJsonGeometry
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    internal class JsonMultipoint : IJsonGeometry
    {
        public double[][] points { get; set; }
    }

    internal class JsonPolyline : IJsonGeometry
    {
        public double[][][] paths { get; set; }
    }

    internal class JsonPolygon : IJsonGeometry
    {
        public double[][][] rings { get; set; }
    }

    #endregion
}
