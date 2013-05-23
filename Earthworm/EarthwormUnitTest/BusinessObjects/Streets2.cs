using System;
using Earthworm;

public class Streets2 : MappableFeature
{
    [MappedField("FULL_NAME", 50)]
    public virtual string FULL_NAME { get; set; }

    [MappedField("CFCC", 3)]
    public virtual string CFCC { get; set; }

    [MappedField("METERS")]
    public virtual double? METERS { get; set; }

    [MappedField("FT_MINUTES")]
    public virtual float? FT_MINUTES { get; set; }

    [MappedField("TF_MINUTES")]
    public virtual float? TF_MINUTES { get; set; }

    [MappedField("DISP_CODE")]
    public virtual short? DISP_CODE { get; set; }

    [MappedField("FUNC_CLASS", 1)]
    public virtual string FUNC_CLASS { get; set; }

    [MappedField("Oneway", 2)]
    public virtual string Oneway { get; set; }

    [MappedField("NA_Hierarc")]
    public virtual int? NA_Hierarc { get; set; }
}
