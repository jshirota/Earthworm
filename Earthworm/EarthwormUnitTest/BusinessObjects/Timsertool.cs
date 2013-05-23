using System;
using Earthworm;

public class Timsertool : MappableFeature
{
    [MappedField("TSDateTime")]
    public virtual DateTime? TSDateTime { get; set; }

    [MappedField("TSValue")]
    public virtual double? TSValue { get; set; }

    [MappedField("GAGE_NO_")]
    public virtual int? GAGE_NO_ { get; set; }

    [MappedField("LONGDD")]
    public virtual double? LONGDD { get; set; }

    [MappedField("LATDD")]
    public virtual double? LATDD { get; set; }

    [MappedField("USGSID", 50)]
    public virtual string USGSID { get; set; }

    [MappedField("Name", 50)]
    public virtual string Name { get; set; }

    [MappedField("year_")]
    public virtual short? year_ { get; set; }
}
