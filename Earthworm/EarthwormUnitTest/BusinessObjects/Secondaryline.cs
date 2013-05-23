using System;
using Earthworm;

public class Secondaryline : MappableFeature
{
    [MappedField("CONDUCTOR", 20)]
    public virtual string CONDUCTOR { get; set; }

    [MappedField("PLACEMENT")]
    public virtual int? PLACEMENT { get; set; }

    [MappedField("PHASECODE")]
    public virtual int? PHASECODE { get; set; }

    [MappedField("ENABLED")]
    public virtual short? ENABLED { get; set; }

    [MappedField("STATIONID", 40)]
    public virtual string STATIONID { get; set; }

    [MappedField("COMMENTS", 255)]
    public virtual string COMMENTS { get; set; }

    [MappedField("UID", 40)]
    public virtual string UID { get; set; }

    [MappedField("SESSIONID", 40)]
    public virtual string SESSIONID { get; set; }

    [MappedField("JOINTTRENCH", 3)]
    public virtual string JOINTTRENCH { get; set; }

    [MappedField("QUANTITY")]
    public virtual int? QUANTITY { get; set; }

    [MappedField("SECTYPE", 20)]
    public virtual string SECTYPE { get; set; }

    [MappedField("NETWORKID", 30)]
    public virtual string NETWORKID { get; set; }

    [MappedField("UNDERBUILT", 3)]
    public virtual string UNDERBUILT { get; set; }

    [MappedField("X")]
    public virtual double? X { get; set; }

    [MappedField("Y")]
    public virtual double? Y { get; set; }
}
