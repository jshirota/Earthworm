using System;
using Earthworm;

public class Primaryline : MappableFeature
{
    [MappedField("PHASECODE")]
    public virtual int? PHASECODE { get; set; }

    [MappedField("PIM", 40)]
    public virtual string PIM { get; set; }

    [MappedField("PLACEMENT")]
    public virtual int? PLACEMENT { get; set; }

    [MappedField("CONDUCTORA", 20)]
    public virtual string CONDUCTORA { get; set; }

    [MappedField("CONDUCTORB", 20)]
    public virtual string CONDUCTORB { get; set; }

    [MappedField("CONDUCTORC", 20)]
    public virtual string CONDUCTORC { get; set; }

    [MappedField("CONDUCTORN", 20)]
    public virtual string CONDUCTORN { get; set; }

    [MappedField("ENABLED")]
    public virtual short? ENABLED { get; set; }

    [MappedField("NETWORKID", 30)]
    public virtual string NETWORKID { get; set; }

    [MappedField("STATIONID", 40)]
    public virtual string STATIONID { get; set; }

    [MappedField("COMMENTS", 255)]
    public virtual string COMMENTS { get; set; }

    [MappedField("UID", 40)]
    public virtual string UID { get; set; }

    [MappedField("SESSIONID", 40)]
    public virtual string SESSIONID { get; set; }

    [MappedField("FACILITYID", 20)]
    public virtual string FACILITYID { get; set; }

    [MappedField("JOINTTRENCH", 3)]
    public virtual string JOINTTRENCH { get; set; }

    [MappedField("OPERVOLT")]
    public virtual double? OPERVOLT { get; set; }

    [MappedField("X")]
    public virtual double? X { get; set; }

    [MappedField("Y")]
    public virtual double? Y { get; set; }

    [MappedField("Feeder", 50)]
    public virtual string Feeder { get; set; }
}
