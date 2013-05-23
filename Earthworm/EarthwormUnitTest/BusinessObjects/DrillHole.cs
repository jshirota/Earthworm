using System;
using Earthworm;

public class DrillHole : MappableFeature
{
    [MappedField("KID", 255)]
    public virtual string KID { get; set; }

    [MappedField("API_NUMBER", 255)]
    public virtual string API_NUMBER { get; set; }

    [MappedField("LONGITUDE", 255)]
    public virtual string LONGITUDE { get; set; }

    [MappedField("LATITUDE", 255)]
    public virtual string LATITUDE { get; set; }

    [MappedField("ELEVATION", 255)]
    public virtual string ELEVATION { get; set; }

    [MappedField("ELEV_REF", 255)]
    public virtual string ELEV_REF { get; set; }

    [MappedField("FORMATION", 255)]
    public virtual string FORMATION { get; set; }

    [MappedField("TOP", 255)]
    public virtual string TOP { get; set; }

    [MappedField("BASE", 255)]
    public virtual string BASE { get; set; }

    [MappedField("SOURCE", 255)]
    public virtual string SOURCE { get; set; }

    [MappedField("UPDATED", 255)]
    public virtual string UPDATED { get; set; }

    [MappedField("KIDN")]
    public virtual int? KIDN { get; set; }
}
