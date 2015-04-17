using System;

namespace Earthworm
{
    /// <summary>
    /// Provides a custom attribute for specifying the source field name.  This is used to map a database field to a property.
    /// </summary>
    public class MappedField : Attribute
    {
        /// <summary>
        /// The function used to retieve the field name.  If this is set to null (default), the text sent to the MappedField constructor is the actual field name.  This can be replaced by another function such as s => ConfigurationManager.AppSettings[s], which will use the string to retrieve the real field name from app.config.
        /// </summary>
        public static Func<string, string> GetFieldName { get; set; }

        /// <summary>
        /// The name of the feature class field.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// The length of the field.  Used by TableWriter to specify the length of a text field when creating a new table.
        /// </summary>
        public int? TextLength { get; private set; }

        /// <summary>
        /// Indicates whether this field should be included in the JSON serialization.
        /// </summary>
        public bool IncludeInJson { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        public MappedField(string fieldName)
            : this(fieldName, 0, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        public MappedField(string fieldName, int textLength)
            : this(fieldName, textLength, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(string fieldName, bool includeInJson)
            : this(fieldName, 0, includeInJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(string fieldName, int textLength, bool includeInJson)
        {
            FieldName = GetFieldName == null ? fieldName : GetFieldName(fieldName);

            if (textLength > 0)
                TextLength = textLength;

            IncludeInJson = includeInJson;
        }
    }
}
