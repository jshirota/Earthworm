using System;
using Earthworm;

public class KSField : MappableFeature
{
    [MappedField("OBJECTID")]
    public virtual int? OBJECTID { get; set; }

    [MappedField("FIELD_KID")]
    public virtual int? FIELD_KID { get; set; }

    [MappedField("FIELD_NAME", 150)]
    public virtual string FIELD_NAME { get; set; }

    [MappedField("PROD_GAS", 3)]
    public virtual string PROD_GAS { get; set; }

    [MappedField("PROD_OIL", 3)]
    public virtual string PROD_OIL { get; set; }

    [MappedField("STATUS", 50)]
    public virtual string STATUS { get; set; }

    [MappedField("DATE_TIME", 50)]
    public virtual string DATE_TIME { get; set; }

    [MappedField("CUMM_OIL")]
    public virtual float? CUMM_OIL { get; set; }

    [MappedField("MAXACTOWEL")]
    public virtual float? MAXACTOWEL { get; set; }

    [MappedField("AVGACTOWEL")]
    public virtual float? AVGACTOWEL { get; set; }

    [MappedField("CUMM_GAS")]
    public virtual float? CUMM_GAS { get; set; }

    [MappedField("MAXACTGWEL")]
    public virtual float? MAXACTGWEL { get; set; }

    [MappedField("AVGACTGWEL")]
    public virtual float? AVGACTGWEL { get; set; }

    [MappedField("AVG_DEPTH")]
    public virtual float? AVG_DEPTH { get; set; }

    [MappedField("A_DEPTH_SL")]
    public virtual float? A_DEPTH_SL { get; set; }

    [MappedField("TOTALACRES")]
    public virtual int? TOTALACRES { get; set; }

    [MappedField("ACRES_10_1")]
    public virtual double? ACRES_10_1 { get; set; }

    [MappedField("ACRES_10_2")]
    public virtual double? ACRES_10_2 { get; set; }
}
