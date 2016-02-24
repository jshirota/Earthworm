using System;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

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
    }
}
