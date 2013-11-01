using System;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.AO
{
    /// <summary>
    /// Provides extension methods for IGeometry so that functions such as spatial predicates can be invoked directly against shape objects.
    /// </summary>
    public static class RelationalOpExt
    {
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
    }
}
