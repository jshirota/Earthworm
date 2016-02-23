using System;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Earthworm
{
    /// <summary>
    /// Provides extension methods for ArcObjects.
    /// </summary>
    public static class AO
    {
        #region Private

        internal static readonly Func<ITable, string, int> GetFieldIndex =
            Memoization.Memoize<ITable, string, int>((table, fieldName) => table.Fields.FindField(fieldName));

        internal static object GetValue(this IRow row, int fieldIndex)
        {
            var value = row.Value[fieldIndex];

            if (value == DBNull.Value)
            {
                return null;
            }

            var fieldType = row.Fields.Field[fieldIndex].Type;

            if (fieldType == esriFieldType.esriFieldTypeBlob)
            {
                var ms = (IMemoryBlobStreamVariant)value;
                ms.ExportToVariant(out value);
            }

            else if (fieldType == esriFieldType.esriFieldTypeGUID)
            {
                value = new Guid((string)value);
            }

            return value;
        }

        internal static object GetValue(this IRow row, string fieldName)
        {
            var fieldIndex = GetFieldIndex(row.Table, fieldName);

            if (fieldIndex == -1)
                throw new MissingFieldException(string.Format("Field '{0}' does not exist in Table '{1}'.", fieldName, ((IDataset)row.Table).Name));

            return row.GetValue(fieldIndex);
        }

        internal static void SetValue(this IRow row, int fieldIndex, object value)
        {
            if (value == null)
            {
                row.Value[fieldIndex] = DBNull.Value;
                return;
            }

            if (value is byte[])
            {
                var ms = (IMemoryBlobStreamVariant)new MemoryBlobStream();
                ms.ImportFromVariant(value);
                value = ms;
            }

            else if (value is Guid)
            {
                value = ((Guid)value).ToString("B").ToUpper();
            }

            row.Value[fieldIndex] = value;
        }

        internal static void SetValue(this IRow row, string fieldName, object value)
        {
            var fieldIndex = GetFieldIndex(row.Table, fieldName);

            if (fieldIndex == -1)
                throw new MissingFieldException(string.Format("Field '{0}' does not exist in Table '{1}'.", fieldName, ((IDataset)row.Table).Name));

            row.SetValue(fieldIndex, value);
        }

        internal static readonly Func<int, ISpatialReference> GetSpatialReference =
            Memoization.Memoize<int, ISpatialReference>(wkid =>
            {
                var spatialReferenceEnvironment = new SpatialReferenceEnvironment();

                try { return spatialReferenceEnvironment.CreateGeographicCoordinateSystem(wkid); }
                catch { return spatialReferenceEnvironment.CreateProjectedCoordinateSystem(wkid); }
            });

        #endregion

        /// <summary>
        /// Encapsulates a workspace edit session (or an edit operation).  The session rolls back if an exception is thrown.  If the workspace is already being edited before this method is called, the edit session will not be stopped at the end of the method.
        /// </summary>
        /// <param name="workspace">The workspace in which edits are performed.</param>
        /// <param name="action">The editing action.</param>
        /// <returns></returns>
        public static void Edit(this IWorkspace workspace, Action action)
        {
            var workspaceEdit = (IWorkspaceEdit)workspace;

            var isBeingEditedAtStart = workspaceEdit.IsBeingEdited();
            var success = false;

            try
            {
                if (!isBeingEditedAtStart)
                    workspaceEdit.StartEditing(false);

                workspaceEdit.StartEditOperation();

                action();

                workspaceEdit.StopEditOperation();

                success = true;
            }
            catch
            {
                workspaceEdit.AbortEditOperation();
                throw;
            }
            finally
            {
                if (workspaceEdit.IsBeingEdited() && !isBeingEditedAtStart)
                    workspaceEdit.StopEditing(success);
            }
        }

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
