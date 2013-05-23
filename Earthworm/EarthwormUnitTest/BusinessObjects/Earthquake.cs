using System;
using Earthworm;


public class Earthquake : MappableFeature
{
    [MappedField("datetime")]
    public virtual DateTime? datetime { get; set; }

    [MappedField("depth")]
    public virtual double? depth { get; set; }

    [MappedField("eqid", 50)]
    public virtual string eqid { get; set; }

    [MappedField("latitude")]
    public virtual double? latitude { get; set; }

    [MappedField("longitude")]
    public virtual double? longitude { get; set; }

    [MappedField("magnitude")]
    public virtual double? magnitude { get; set; }

    [MappedField("numstations")]
    public virtual int? numstations { get; set; }

    [MappedField("region", 200)]
    public virtual string region { get; set; }

    [MappedField("source", 50)]
    public virtual string source { get; set; }

    [MappedField("version", 50)]
    public virtual string version { get; set; }
}

