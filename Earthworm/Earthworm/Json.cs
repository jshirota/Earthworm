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

        private static SpatialReference Convert(this ISpatialReference spatialReference)
        {
            if (spatialReference == null)
                return null;

            return new SpatialReference { wkid = spatialReference.FactoryCode };
        }

        private static ISpatialReference Convert(this SpatialReference spatialReference)
        {
            if (spatialReference == null)
                return null;

            return Shape.WKID(spatialReference.wkid);
        }

        private static JsonPoint ToPoint(this IPoint shape)
        {
            return new JsonPoint { x = shape.X, y = shape.Y, spatialReference = shape.SpatialReference.Convert() };
        }

        private static JsonMultipoint ToMultipoint(this IMultipoint shape)
        {
            return new JsonMultipoint { points = ((IPointCollection4)shape).ToArray(), spatialReference = shape.SpatialReference.Convert() };
        }

        private static JsonPolyline ToPolyline(this IPolyline shape)
        {
            return new JsonPolyline { paths = ((IGeometryCollection)shape).ToArray(), spatialReference = shape.SpatialReference.Convert() };
        }

        private static JsonPolygon ToPolygon(this IPolygon shape)
        {
            return new JsonPolygon { rings = ((IGeometryCollection)shape).ToArray(), spatialReference = shape.SpatialReference.Convert() };
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
            return new Point { X = shape.x, Y = shape.y, SpatialReference = shape.spatialReference.Convert() };
        }

        /// <summary>
        /// Converts a JSON string into an Esri IMultipoint object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IMultipoint ToMultipoint(string json)
        {
            var shape = json.Deserialize<JsonMultipoint>();
            var multipoint = (IMultipoint)new Multipoint();
            multipoint.SpatialReference = shape.spatialReference.Convert();
            return multipoint.Load(shape.points);
        }

        /// <summary>
        /// Converts a JSON string into an Esri IPolyline object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IPolyline ToPolyline(string json)
        {
            var shape = json.Deserialize<JsonPolyline>();
            var polyline = (IPolyline)new Polyline();
            polyline.SpatialReference = shape.spatialReference.Convert();

            foreach (var path in shape.paths)
                ((IGeometryCollection)polyline).AddGeometry(((IGeometry)new Path()).Load(path));

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
            var polygon = (IPolygon)new Polygon();
            polygon.SpatialReference = shape.spatialReference.Convert();

            foreach (var ring in shape.rings)
                ((IGeometryCollection)polygon).AddGeometry(((IGeometry)new Ring()).Load(ring));

            return polygon;
        }
    }

    #region Types

    internal interface IJsonGeometry
    {
    }

    internal class JsonGeometry : IJsonGeometry
    {
        public SpatialReference spatialReference { get; set; }
    }

    internal class SpatialReference
    {
        public int wkid { get; set; }
    }

    internal class JsonPoint : JsonGeometry
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    internal class JsonMultipoint : JsonGeometry
    {
        public double[][] points { get; set; }
    }

    internal class JsonPolyline : JsonGeometry
    {
        public double[][][] paths { get; set; }
    }

    internal class JsonPolygon : JsonGeometry
    {
        public double[][][] rings { get; set; }
    }

    #endregion
}
