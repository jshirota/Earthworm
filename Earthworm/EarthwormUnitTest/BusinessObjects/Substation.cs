using System;
using Earthworm;

public class Substation : MappableFeature
{
    [MappedField("FACILITYID", 20)]
    public virtual string FACILITYID { get; set; }

    [MappedField("NAME", 50)]
    public virtual string NAME { get; set; }

    [MappedField("PIM", 40)]
    public virtual string PIM { get; set; }

    [MappedField("PHASECODE")]
    public virtual int? PHASECODE { get; set; }

    [MappedField("ANCILLARYROLE")]
    public virtual short? ANCILLARYROLE { get; set; }

    [MappedField("ROTATION")]
    public virtual double? ROTATION { get; set; }

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

    [MappedField("BUSVOLTAGE")]
    public virtual double? BUSVOLTAGE { get; set; }

    [MappedField("NOMVOLTAGE")]
    public virtual double? NOMVOLTAGE { get; set; }

    [MappedField("LOADCONNECTION", 5)]
    public virtual string LOADCONNECTION { get; set; }

    [MappedField("SOURCEREG", 15)]
    public virtual string SOURCEREG { get; set; }

    [MappedField("ZSMIMPDESCMIN", 20)]
    public virtual string ZSMIMPDESCMIN { get; set; }

    [MappedField("ZSMIMPDESCMAX", 20)]
    public virtual string ZSMIMPDESCMAX { get; set; }

    [MappedField("OHGROUNDOHMS")]
    public virtual int? OHGROUNDOHMS { get; set; }

    [MappedField("UGGROUNDOHMS")]
    public virtual int? UGGROUNDOHMS { get; set; }

    [MappedField("SUBSTATIONID", 14)]
    public virtual string SUBSTATIONID { get; set; }

    [MappedField("SCHEMATIC", 255)]
    public virtual string SCHEMATIC { get; set; }

    [MappedField("CAPACITY")]
    public virtual double? CAPACITY { get; set; }

    [MappedField("X")]
    public virtual double? X { get; set; }

    [MappedField("Y")]
    public virtual double? Y { get; set; }

    [MappedField("Z")]
    public virtual double? Z { get; set; }
}
