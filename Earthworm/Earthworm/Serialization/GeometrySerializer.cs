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

        private static double[][] ToArray(this IPointCollection4 points)
        {
            return Enumerable.Range(0, points.PointCount).Select(i =>
            {
                IPoint p = points.Point[i];
                return new[] { p.X, p.Y };
            }).ToArray();
        }

        private static double[][][] ToArray(this IGeometryCollection collection)
        {
            return Enumerable.Range(0, collection.GeometryCount)
                .Select(i => ((IPointCollection4)collection.Geometry[i]).ToArray())
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

        private static IPoint ToEsriPoint(this JsonPoint shape)
        {
            return new Point { X = shape.x, Y = shape.y };
        }

        private static IMultipoint ToEsriMultipoint(this JsonMultipoint shape)
        {
            IPointCollection4 multipoint = (IPointCollection4)new Multipoint();
            IGeometryBridge2 geometryEnvironment = (IGeometryBridge2)new GeometryEnvironment();

            WKSPoint[] points = shape.points.Select(c => new WKSPoint { X = c[0], Y = c[1] }).ToArray();
            geometryEnvironment.SetWKSPoints(multipoint, ref points);

            return (IMultipoint)multipoint;
        }

        private static IPolyline ToEsriPolyline(this JsonPolyline shape)
        {
            IGeometryCollection polyline = (IGeometryCollection)new Polyline();
            IGeometryBridge2 geometryEnvironment = (IGeometryBridge2)new GeometryEnvironment();

            foreach (double[][] p in shape.paths)
            {
                IPointCollection4 path = (IPointCollection4)new Path();
                WKSPoint[] points = p.Select(c => new WKSPoint { X = c[0], Y = c[1] }).ToArray();
                geometryEnvironment.SetWKSPoints(path, ref points);
                polyline.AddGeometry((IGeometry)path);
            }

            return (IPolyline)polyline;
        }

        private static IPolygon ToEsriPolygon(this JsonPolygon shape)
        {
            IGeometryCollection polygon = (IGeometryCollection)new Polygon();
            IGeometryBridge2 geometryEnvironment = (IGeometryBridge2)new GeometryEnvironment();

            foreach (double[][] r in shape.rings)
            {
                IPointCollection4 ring = (IPointCollection4)new Ring();
                WKSPoint[] points = r.Select(c => new WKSPoint { X = c[0], Y = c[1] }).ToArray();
                geometryEnvironment.SetWKSPoints(ring, ref points);
                polygon.AddGeometry((IGeometry)ring);
            }

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
            if (shape is IPoint)
                return ((IPoint)shape).ToPoint();

            if (shape is IMultipoint)
                return ((IMultipoint)shape).ToMultipoint();

            if (shape is IPolyline)
                return ((IPolyline)shape).ToPolyline();

            if (shape is IPolygon)
                return ((IPolygon)shape).ToPolygon();

            throw new Exception("This geometry type is not supported.");
        }

        /// <summary>
        /// Converts an IJsonGeometry object into an Esri IGeometry object.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static IGeometry ToEsriGeometry(this IJsonGeometry shape)
        {
            if (shape is JsonPoint)
                return ((JsonPoint)shape).ToEsriPoint();

            if (shape is JsonMultipoint)
                return ((JsonMultipoint)shape).ToEsriMultipoint();

            if (shape is JsonPolyline)
                return ((JsonPolyline)shape).ToEsriPolyline();

            if (shape is JsonPolygon)
                return ((JsonPolygon)shape).ToEsriPolygon();

            throw new Exception("This geometry type is not supported.");
        }
    }
}
