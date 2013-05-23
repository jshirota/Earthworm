using System;
using Earthworm;

public class MetroLine : MappableFeature
{
    [MappedField("NAME", 120)]
    public virtual string NAME { get; set; }

    [MappedField("INDICE", 8)]
    public virtual string INDICE { get; set; }

    [MappedField("ID_LINE")]
    public virtual short? ID_LINE { get; set; }

    [MappedField("TransitTim")]
    public virtual double? TransitTim { get; set; }

    [MappedField("Meters")]
    public virtual double? Meters { get; set; }
}
