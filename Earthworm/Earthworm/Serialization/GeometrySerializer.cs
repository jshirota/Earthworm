using System;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.Serialization
{
    /// <summary>
    /// Provides extension methods for converting geometries into JSON-serializable objects.
    /// </summary>
    public static class GeometrySerializer
    {
        #region Private

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

        private static IGeometry Load<T>(this T shape, double[][] array)
        {
            var points = array.Select(c => new WKSPoint { X = c[0], Y = c[1] }).ToArray();
            ((IGeometryBridge2)new GeometryEnvironment()).SetWKSPoints((IPointCollection4)shape, ref points);

            return (IGeometry)shape;
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

        private static IPoint ToEsriPoint(this JsonPoint shape)
        {
            return new Point { X = shape.x, Y = shape.y };
        }

        private static IMultipoint ToEsriMultipoint(this JsonMultipoint shape)
        {
            return (IMultipoint)new Multipoint().Load(shape.points);
        }

        private static IPolyline ToEsriPolyline(this JsonPolyline shape)
        {
            var polyline = (IGeometryCollection)new Polyline();

            foreach (var path in shape.paths)
                polyline.AddGeometry(new Path().Load(path));

            return (IPolyline)polyline;
        }

        private static IPolygon ToEsriPolygon(this JsonPolygon shape)
        {
            var polygon = (IGeometryCollection)new Polygon();

            foreach (var ring in shape.rings)
                polygon.AddGeometry(new Ring().Load(ring));

            return (IPolygon)polygon;
        }

        #endregion

        /// <summary>
        /// Converts an Esri IGeometry object into an IJsonGeometry object.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IJsonGeometry ToJsonGeometry(this IGeometry shape)
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

            throw new Exception("This geometry type is not supported.");
        }

        /// <summary>
        /// Converts an IJsonGeometry object into an Esri IGeometry object.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGeometry ToEsriGeometry(this IJsonGeometry shape)
        {
            var point = shape as JsonPoint;
            if (point != null)
                return point.ToEsriPoint();

            var multipoint = shape as JsonMultipoint;
            if (multipoint != null)
                return multipoint.ToEsriMultipoint();

            var polyline = shape as JsonPolyline;
            if (polyline != null)
                return polyline.ToEsriPolyline();

            var polygon = shape as JsonPolygon;
            if (polygon != null)
                return polygon.ToEsriPolygon();

            throw new Exception("This geometry type is not supported.");
        }
    }
}
