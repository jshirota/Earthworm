using System.Collections.Concurrent;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.AO
{
    /// <summary>
    /// Provides extension methods for IGeometry so that functions such as topological operations can be invoked directly against shape objects.
    /// </summary>
    public static class TopologicalOpExt
    {
        private static readonly ConcurrentDictionary<int, ISpatialReference> WkidToSpatialReference = new ConcurrentDictionary<int, ISpatialReference>();

        #region Private

        internal static ISpatialReference GetSpatialReference(int wkid)
        {
            return WkidToSpatialReference.GetOrAdd(wkid, n =>
            {
                var spatialReferenceEnvironment = new SpatialReferenceEnvironment();

                try { return spatialReferenceEnvironment.CreateGeographicCoordinateSystem(wkid); }
                catch { return spatialReferenceEnvironment.CreateProjectedCoordinateSystem(wkid); }
            });
        }

        internal static ISpatialReference GetSpatialReference(string wkt)
        {
            var spatialReferenceEnvironment = new SpatialReferenceEnvironment();

            ISpatialReference spatialReference;
            int n;

            spatialReferenceEnvironment.CreateESRISpatialReference(wkt, out spatialReference, out n);

            return spatialReference;
        }

        #endregion

        /// <summary>
        /// Returns a buffered shape.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the current geometry.</param>
        /// <param name="autoDensify">If set to true, the resulting polygon is densified.  Use this to convert a circle to a polygon with vertices.</param>
        /// <returns></returns>
        public static IGeometry Buffer(this IGeometry shape, double distance, bool autoDensify)
        {
            var result = ((ITopologicalOperator)shape).Buffer(distance);

            if (autoDensify)
                ((IPolygon)result).Densify(-1, -1);

            return result;
        }

        /// <summary>
        /// Returns a buffered shape.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the current geometry.</param>
        /// <returns></returns>
        public static IGeometry Buffer(this IGeometry shape, double distance)
        {
            return shape.Buffer(distance, false);
        }

        /// <summary>
        /// Returns a buffered shape.  The shape is buffered in the specified spatial reference.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the specified spatial reference.</param>
        /// <param name="spatialReference">The spatial reference in which the distance is calculated.</param>
        /// <param name="autoDensify">If set to true, the resulting polygon is densified.  Use this to convert a circle to a polygon with vertices.</param>
        /// <returns></returns>
        public static IGeometry Buffer(this IGeometry shape, double distance, ISpatialReference spatialReference, bool autoDensify)
        {
            var originalSpatialReference = shape.SpatialReference;
            return shape.Project2(spatialReference).Buffer(distance, autoDensify).Project2(originalSpatialReference);
        }

        /// <summary>
        /// Returns a buffered shape.  The shape is buffered in the specified spatial reference.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the specified spatial reference.</param>
        /// <param name="wkid">The well-known ID of the spatial reference in which the distance is calculated.</param>
        /// <param name="autoDensify">If set to true, the resulting polygon is densified.  Use this to convert a circle to a polygon with vertices.</param>
        /// <returns></returns>
        public static IGeometry Buffer(this IGeometry shape, double distance, int wkid, bool autoDensify)
        {
            var spatialReference = GetSpatialReference(wkid);
            return shape.Buffer(distance, spatialReference, autoDensify);
        }

        /// <summary>
        /// Returns the boundary of this geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <returns></returns>
        public static IGeometry Boundary(this IGeometry shape)
        {
            return ((ITopologicalOperator)shape).Boundary;
        }

        /// <summary>
        /// Returns the convex hull of this geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <returns></returns>
        public static IGeometry ConvexHull(this IGeometry shape)
        {
            return ((ITopologicalOperator)shape).ConvexHull();
        }

        /// <summary>
        /// Clones the current geometry without altering the state of the original object.
        /// </summary>
        /// <typeparam name="T">The type of geometry.</typeparam>
        /// <param name="shape">The current geometry.</param>
        /// <returns>A new copy of the geometry.</returns>
        public static T Copy<T>(this T shape) where T : class, IGeometry
        {
            if (shape == null)
                return default(T);

            var clone = (IClone)shape;
            return (T)clone.Clone();
        }

        /// <summary>
        /// Returns the intersection of this geometry and the specified envelope.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="clipperEnvelope">The "cookie cutter" geometry.</param>
        /// <returns></returns>
        public static IGeometry Clip(this IGeometry shape, IEnvelope clipperEnvelope)
        {
            var clone = shape.Copy();
            ((ITopologicalOperator)shape).Clip(clipperEnvelope);
            return clone;
        }

        /// <summary>
        /// Returns the geometry containing points from this geometry but not the other geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="other">The other geometry.</param>
        /// <returns></returns>
        public static IGeometry Difference(this IGeometry shape, IGeometry other)
        {
            return ((ITopologicalOperator)shape).Difference(other);
        }

        /// <summary>
        /// Returns the geometry that is the set-theoretic intersection of the input geometries.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="other">The other geometry.</param>
        /// <returns></returns>
        public static IGeometry Intersect(this IGeometry shape, IGeometry other)
        {
            return ((ITopologicalOperator)shape).Intersect(other, esriGeometryDimension.esriGeometryNoDimension);
        }

        /// <summary>
        /// Returns the geometry that is the set-theoretic union of the input geometries.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="other">The other geometry.</param>
        /// <returns></returns>
        public static IGeometry Union(this IGeometry shape, IGeometry other)
        {
            return ((ITopologicalOperator)shape).Union(other);
        }

        /// <summary>
        /// Returns a projected copy of the geometry without altering the state of the original object.
        /// </summary>
        /// <typeparam name="T">The type of geometry.</typeparam>
        /// <param name="shape">The current geometry.</param>
        /// <param name="spatialReference">The target spatial reference.</param>
        /// <param name="transformation">The datum transformation.</param>
        /// <param name="direction">The direction of transformation.</param>
        /// <returns>A projected copy of the geometry.</returns>
        public static T Project2<T>(this T shape, ISpatialReference spatialReference, IGeoTransformation transformation, esriTransformDirection direction) where T : class, IGeometry
        {
            var copy = shape.Copy();

            if (transformation == null)
                copy.Project(spatialReference);
            else
                ((IGeometry2)copy).ProjectEx(spatialReference, direction, transformation, false, 0, 0);

            return copy;
        }

        /// <summary>
        /// Returns a projected copy of the geometry without altering the state of the original object.
        /// </summary>
        /// <typeparam name="T">The type of geometry.</typeparam>
        /// <param name="shape">The current geometry.</param>
        /// <param name="spatialReference">The target spatial reference.</param>
        /// <returns>A projected copy of the geometry.</returns>
        public static T Project2<T>(this T shape, ISpatialReference spatialReference) where T : class, IGeometry
        {
            return shape.Project2(spatialReference, null, esriTransformDirection.esriTransformForward);
        }

        /// <summary>
        /// Returns a projected copy of the geometry without altering the state of the  original object.
        /// </summary>
        /// <typeparam name="T">The type of geometry.</typeparam>
        /// <param name="shape">The current geometry.</param>
        /// <param name="wkid">The well-known ID of the target spatial reference (i.e. 4326 for WGS84).</param>
        /// <returns>A projected copy of the geometry.</returns>
        public static T Project2<T>(this T shape, int wkid) where T : class, IGeometry
        {
            return shape.Project2(GetSpatialReference(wkid));
        }
    }
}
