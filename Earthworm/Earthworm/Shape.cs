using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides functions for succinctly constructing and manipulating geometry objects.  Use this in conjunction with the using static directive.
    /// </summary>
    public static class Shape
    {
        #region Private

        private static readonly Func<int, ISpatialReference> GetSpatialReference =
            Memoization.Memoize<int, ISpatialReference>(wkid =>
            {
                var spatialReferenceEnvironment = new SpatialReferenceEnvironment();

                try { return spatialReferenceEnvironment.CreateGeographicCoordinateSystem(wkid); }
                catch { return spatialReferenceEnvironment.CreateProjectedCoordinateSystem(wkid); }
            });

        internal static T Load<T>(this T shape, double[][] array) where T : IPointCollection4
        {
            var points = array.Select(c => new WKSPointZ { X = c[0], Y = c[1], Z = c.ElementAtOrDefault(2) }).ToArray();
            new GeometryEnvironmentClass().SetWKSPointZs(shape, ref points);

            return shape;
        }

        private static T Load<T>(this T shape, IEnumerable<IPoint> points) where T : IPointCollection4
        {
            return shape.Load(points.Select(p => new[] { p.X, p.Y }).ToArray());
        }

        internal static bool IsInnerRing(this double[][] ring)
        {
            return Enumerable.Range(0, ring.Length - 1)
                .Sum(i => ring[i][0] * ring[i + 1][1] - ring[i + 1][0] * ring[i][1]) > 0;
        }

        private static bool IsInnerRing(this IEnumerable<IPoint> points)
        {
            return points.Select(p => new[] { p.X, p.Y }).ToArray().IsInnerRing();
        }

        #endregion

        #region Spatial Reference

        /// <summary>
        /// The default spatial reference used when constructing a Point object.
        /// </summary>
        public static ISpatialReference DefaultSpatialReference { get; set; }

        /// <summary>
        /// Creates a spatial reference from a Well-Known ID.
        /// </summary>
        /// <param name="wkid"></param>
        /// <returns></returns>
        public static ISpatialReference WKID(int wkid)
        {
            return GetSpatialReference(wkid);
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
            return shape.Project2(WKID(wkid));
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Point class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="spatialReference"></param>
        /// <returns></returns>
        public static IPoint P(double x, double y, double z, ISpatialReference spatialReference = null)
        {
            return new PointClass { X = x, Y = y, Z = z, SpatialReference = spatialReference ?? DefaultSpatialReference };
        }

        /// <summary>
        /// Initializes a new instance of the Point class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="spatialReference"></param>
        /// <returns></returns>
        public static IPoint P(double x, double y, ISpatialReference spatialReference = null)
        {
            return P(x, y, 0, spatialReference);
        }

        /// <summary>
        /// Initializes a new instance of the Multipoint class.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IMultipoint Multipoint(params IPoint[] points)
        {
            return new MultipointClass().Load(points);
        }

        /// <summary>
        /// Initializes a new instance of the Path class.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPath Path(params IPoint[] points)
        {
            return new PathClass().Load(points);
        }

        /// <summary>
        /// Initializes a new instance of the Polyline class.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IPolyline Polyline(params IPath[] paths)
        {
            var polyline = new PolylineClass();

            foreach (var path in paths)
                polyline.AddGeometry(path);

            return polyline;
        }

        /// <summary>
        /// Initializes a new instance of the Polyline class.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPolyline Polyline(params IPoint[] points)
        {
            return Polyline(Path(points));
        }

        /// <summary>
        /// Initializes a new instance of the Ring class.  Reorders the points clockwise (if not already).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IRing OuterRing(params IPoint[] points)
        {
            return new RingClass().Load(points.IsInnerRing() ? points.Reverse() : points);
        }

        /// <summary>
        /// Initializes a new instance of the Ring class.  Reorders the points counterclockwise (if not already).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IRing InnerRing(params IPoint[] points)
        {
            return new RingClass().Load(points.IsInnerRing() ? points : points.Reverse());
        }

        /// <summary>
        /// Initializes a new instance of the Polygon class.
        /// </summary>
        /// <param name="rings"></param>
        /// <returns></returns>
        public static IPolygon Polygon(params IRing[] rings)
        {
            var polygon = new PolygonClass();

            foreach (var ring in rings)
                polygon.AddGeometry(ring);

            return polygon;
        }

        /// <summary>
        /// Initializes a new instance of the Polygon class.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPolygon Polygon(params IPoint[] points)
        {
            return Polygon(OuterRing(points));
        }

        #endregion

        #region Predicates

        /// <summary>
        /// Indicates if this geometry contains the other geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Contains(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Contains(comparisonShape);
        }

        /// <summary>
        /// Indicates if the two geometries intersect in a geometry of lesser dimension.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Crosses(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Crosses(comparisonShape);
        }

        /// <summary>
        /// Indicates if the two geometries share no points in common.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Disjoint(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Disjoint(comparisonShape);
        }

        /// <summary>
        /// Indicates if the two geometries are of the same type and define the same set of points in the plane.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Equals2(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Equals(comparisonShape);
        }

        /// <summary>
        /// Indicates if the two points are the same.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Equals2(this IPoint shape, IPoint comparisonShape)
        {
            var tolerance = 0.001;

            return Math.Abs(shape.X - comparisonShape.X) < tolerance
                && Math.Abs(shape.Y - comparisonShape.Y) < tolerance
                && ((IRelationalOperator)shape).Equals(comparisonShape);
        }

        /// <summary>
        /// Indicates if this geometry intersects the other geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Intersects(this IGeometry shape, IGeometry comparisonShape)
        {
            return !shape.Disjoint(comparisonShape);
        }

        /// <summary>
        /// Indicates if the intersection of the two geometries has the same dimension as one of the input geometries.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Overlaps(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Overlaps(comparisonShape);
        }

        /// <summary>
        /// Indicates if the defined relationship exists.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <param name="relationDescription">Description of the spatial relation (Shape Comparison Language).</param>
        /// <returns>True or false.</returns>
        public static bool Relation(this IGeometry shape, IGeometry comparisonShape, string relationDescription)
        {
            return ((IRelationalOperator)shape).Relation(comparisonShape, relationDescription);
        }

        /// <summary>
        /// Indicates if the boundaries of the geometries intersect.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Touches(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Touches(comparisonShape);
        }

        /// <summary>
        /// Indicates if this geometry is contained (is within) another geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns>True or false.</returns>
        public static bool Within(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IRelationalOperator)shape).Within(comparisonShape);
        }

        /// <summary>
        /// Indicates if this geometry is within the distance of (buffer intersects) another geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <param name="distance">The distance in the map unit of the current shape.</param>
        /// <returns>True or false.</returns>
        public static bool Within(this IGeometry shape, IGeometry comparisonShape, double distance)
        {
            return shape.Buffer(distance).Intersects(comparisonShape);
        }

        #endregion

        #region Operations

        /// <summary>
        /// Calculates the distance from the current geometry to the other geometry.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="comparisonShape">The geometry to compare with.</param>
        /// <returns></returns>
        public static double DistanceTo(this IGeometry shape, IGeometry comparisonShape)
        {
            return ((IProximityOperator)shape).ReturnDistance(comparisonShape);
        }

        /// <summary>
        /// Returns a buffered shape.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the current geometry.</param>
        /// <param name="autoDensify">If set to true, the resulting polygon is densified.  Use this to convert a circle to a polygon with vertices.</param>
        /// <returns></returns>
        public static IPolygon Buffer(this IGeometry shape, double distance, bool autoDensify)
        {
            var result = ((ITopologicalOperator)shape).Buffer(distance);

            if (autoDensify)
                ((IPolygon)result).Densify(-1, -1);

            return (IPolygon)result;
        }

        /// <summary>
        /// Returns a buffered shape.
        /// </summary>
        /// <param name="shape">The current geometry.</param>
        /// <param name="distance">The distance in the map unit of the current geometry.</param>
        /// <returns></returns>
        public static IPolygon Buffer(this IGeometry shape, double distance)
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
        public static IPolygon Buffer(this IGeometry shape, double distance, ISpatialReference spatialReference, bool autoDensify)
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
        public static IPolygon Buffer(this IGeometry shape, double distance, int wkid, bool autoDensify)
        {
            var spatialReference = WKID(wkid);
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

        #endregion
    }
}
