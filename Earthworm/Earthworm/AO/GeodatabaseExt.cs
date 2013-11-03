using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;

namespace Earthworm.AO
{
    /// <summary>
    /// Provides extension methods for ArcObjects interfaces related to geodatabase.
    /// </summary>
    public static class GeodatabaseExt
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
    }
}
