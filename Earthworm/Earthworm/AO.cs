using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        internal static IEnumerable<IRow> ReadRows(this ITable table, IQueryFilter filter)
        {
            var cursor = table.Search(filter, false);

            try
            {
                IRow row;

                while (true)
                {
                    row = cursor.NextRow();

                    if (row == null)
                        yield break;

                    yield return row;
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(cursor) != 0) { }
            }
        }

        internal static object GetValue(this IRow row, int fieldIndex)
        {
            var o = row.Value[fieldIndex];
            return o == DBNull.Value ? null : o;
        }

        internal static void SetValue(this IRow row, int fieldIndex, object value)
        {
            row.Value[fieldIndex] = value ?? DBNull.Value;
        }

        #endregion

        /// <summary>
        /// Encapsulates a workspace edit session (or an edit operation).  The edits will not be saved if an exception is thrown.  If the workspace is already being edited before this method is called, the edit session will not be stopped at the end of the method.
        /// </summary>
        /// <param name="workspace">The workspace in which edits are performed.</param>
        /// <param name="action">The editing action.</param>
        /// <param name="exception">Any exception that may be thrown within the edit session.</param>
        /// <returns></returns>
        public static bool Edit(this IWorkspace workspace, Action action, out Exception exception)
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
                exception = null;
            }
            catch (Exception ex)
            {
                workspaceEdit.AbortEditOperation();
                exception = ex;
            }
            finally
            {
                if (workspaceEdit.IsBeingEdited() && !isBeingEditedAtStart)
                    workspaceEdit.StopEditing(success);
            }

            return success;
        }

        /// <summary>
        /// Encapsulates a workspace edit session (or an edit operation).  The edits will not be saved if an exception is thrown.  If the workspace is already being edited before this method is called, the edit session will not be stopped at the end of the method.
        /// </summary>
        /// <param name="workspace">The workspace in which edits are performed.</param>
        /// <param name="action">The editing action.</param>
        /// <returns></returns>
        public static bool Edit(this IWorkspace workspace, Action action)
        {
            Exception ex;
            return workspace.Edit(action, out ex);
        }

        /// <summary>
        /// Opens an existing table.  If the table with the specified name does not exist, returns null.
        /// </summary>
        /// <param name="featureWorkspace"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ITable OpenTable2(this IFeatureWorkspace featureWorkspace, string tableName)
        {
            try { return featureWorkspace.OpenTable(tableName); }
            catch { return null; }
        }

        /// <summary>
        /// Opens an existing feature class.  If the feature class with the specified name does not exist, returns null.
        /// </summary>
        /// <param name="featureWorkspace"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass2(this IFeatureWorkspace featureWorkspace, string featureClassName)
        {
            try { return featureWorkspace.OpenFeatureClass(featureClassName); }
            catch { return null; }
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
