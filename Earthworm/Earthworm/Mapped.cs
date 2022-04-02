namespace Earthworm;

/// <summary>
/// Provides a custom attribute for specifying the source field name.  This is used to map a database field to a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Mapped : Attribute
{
    /// <summary>
    /// The function used to retieve the field name.  If this is set to null (default), the text sent to the Mapped constructor is the actual field name.  This can be replaced by another function such as s => ConfigurationManager.AppSettings[s], which will use the string to retrieve the real field name from app.config.
    /// </summary>
    public static Func<string, string>? GetFieldName { get; set; }

    /// <summary>
    /// The name of the database field represented by this property.
    /// </summary>
    public string FieldName { get; private set; }

    /// <summary>
    /// The length of the field.  Used by TableWriter to specify the length of a text field when creating a new table.
    /// </summary>
    public int? Length { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Mapped class.
    /// </summary>
    /// <param name="fieldName"></param>    
    public Mapped(string fieldName)
    {
        FieldName = GetFieldName == null ? fieldName : GetFieldName(fieldName);
    }

    /// <summary>
    /// Initializes a new instance of the Mapped class.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="length"></param>
    public Mapped(string fieldName, int length)
        : this(fieldName)
    {
        Length = length;
    }
}
