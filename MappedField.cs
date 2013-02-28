using System;

namespace Earthworm
{
    /// <summary>
    /// Provides a custom attribute for specifying the source field name.  This is used to map a database field to a property.
    /// </summary>
    public class MappedField : Attribute
    {
        /// <summary>
        /// The name of the feature class field.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// The length of the field.  Used by TableWriter to specify the length of a text field when creating a new table.
        /// </summary>
        public int? TextLength { get; private set; }

        /// <summary>
        /// Indicates whether the underlying field is updatable or not.
        /// </summary>
        public bool IsWritable { get; private set; }

        /// <summary>
        /// Indicates whether this field should be included in the JSON serialization.
        /// </summary>
        public bool IncludeInJson { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        public MappedField(string fieldName)
            : this(fieldName, 0, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        public MappedField(string fieldName, int textLength)
            : this(fieldName, textLength, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="isWritable">Indicates whether the underlying field is updatable or not.</param>
        public MappedField(string fieldName, bool isWritable)
            : this(fieldName, 0, isWritable, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(string fieldName, int textLength, bool includeInJson)
            : this(fieldName, textLength, true, includeInJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class.
        /// </summary>
        /// <param name="fieldName">The name of the table field.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        /// <param name="isWritable">Indicates whether the underlying field is updatable or not.</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(string fieldName, int textLength, bool isWritable, bool includeInJson)
        {
            FieldName = fieldName;

            if (textLength > 0)
                TextLength = textLength;

            IsWritable = isWritable;
            IncludeInJson = includeInJson;
        }
    }
}
