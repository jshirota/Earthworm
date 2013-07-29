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
                IPoint p = pointCollection.Point[i];
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
            WKSPoint[] points = array.Select(c => new WKSPoint { X = c[0], Y = c[1] }).ToArray();
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
            IGeometryCollection polyline = (IGeometryCollection)new Polyline();

            foreach (double[][] path in shape.paths)
                polyline.AddGeometry(new Path().Load(path));

            return (IPolyline)polyline;
        }

        private static IPolygon ToEsriPolygon(this JsonPolygon shape)
        {
            IGeometryCollection polygon = (IGeometryCollection)new Polygon();

            foreach (double[][] ring in shape.rings)
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
