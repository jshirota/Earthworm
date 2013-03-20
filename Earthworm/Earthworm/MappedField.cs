using System;
using System.Configuration;
using System.Reflection;

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
            FieldName = fieldName;

            if (textLength > 0)
                TextLength = textLength;

            IncludeInJson = includeInJson;
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class based on the application settings.
        /// </summary>
        /// <param name="settingsType">The type that provides the application settings.  This may be System.Configuration.ConfigurationManager (i.e. typeof(ConfigurationManager)).  It can also be a type derived from System.Configuration.SettingsBase (i.e. typeof(MyNamespace.Properties.Settings)).  In this case, the type must contain a static property called "Default", which returns an instance of the type.</param>
        /// <param name="name">The name of the configuration setting that represents the field name.</param>
        public MappedField(Type settingsType, string name)
            : this(settingsType, name, 0, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class based on the application settings.
        /// </summary>
        /// <param name="settingsType">The type that provides the application settings.  This may be System.Configuration.ConfigurationManager (i.e. typeof(ConfigurationManager)).  It can also be a type derived from System.Configuration.SettingsBase (i.e. typeof(MyNamespace.Properties.Settings)).  In this case, the type must contain a static property called "Default", which returns an instance of the type.</param>
        /// <param name="name">The name of the configuration setting that represents the field name.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        public MappedField(Type settingsType, string name, int textLength)
            : this(settingsType, name, textLength, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class based on the application settings.
        /// </summary>
        /// <param name="settingsType">The type that provides the application settings.  This may be System.Configuration.ConfigurationManager (i.e. typeof(ConfigurationManager)).  It can also be a type derived from System.Configuration.SettingsBase (i.e. typeof(MyNamespace.Properties.Settings)).  In this case, the type must contain a static property called "Default", which returns an instance of the type.</param>
        /// <param name="name">The name of the configuration setting that represents the field name.</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(Type settingsType, string name, bool includeInJson)
            : this(settingsType, name, 0, includeInJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MappedField class based on the application settings.
        /// </summary>
        /// <param name="settingsType">The type that provides the application settings.  This may be System.Configuration.ConfigurationManager (i.e. typeof(ConfigurationManager)).  It can also be a type derived from System.Configuration.SettingsBase (i.e. typeof(MyNamespace.Properties.Settings)).  In this case, the type must contain a static property called "Default", which returns an instance of the type.</param>
        /// <param name="name">The name of the configuration setting that represents the field name.</param>
        /// <param name="textLength">The length of the field (for text fields only).</param>
        /// <param name="includeInJson">Indicates whether this field should be included in the JSON serialization.</param>
        public MappedField(Type settingsType, string name, int textLength, bool includeInJson)
            : this(GetFieldName(settingsType, name), textLength, includeInJson)
        {
        }

        #region Private

        private static string GetFieldName(Type settingsType, string name)
        {
            if (settingsType == typeof(ConfigurationManager))
            {
                string fieldName = ConfigurationManager.AppSettings[name];

                if (fieldName == null)
                    throw new Exception(string.Format("'{0}' does not exist in the application settings.", name));

                return fieldName;
            }

            PropertyInfo property = settingsType.GetProperty("Default");

            SettingsBase settings;

            if (property == null || (settings = property.GetValue(null, null) as SettingsBase) == null)
                throw new Exception(string.Format("'{0}' does not contain any application settings.", settingsType.FullName));

            return settings[name] as string;
        }

        #endregion
    }
}
