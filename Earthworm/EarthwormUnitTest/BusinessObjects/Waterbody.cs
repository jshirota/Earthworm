using System;
using Earthworm;

public class Waterbody : MappableFeature
{
    [MappedField("AREA")]
    public virtual float? AREA { get; set; }

    [MappedField("PERIMETER")]
    public virtual float? PERIMETER { get; set; }

    [MappedField("WATERBODY_COV_")]
    public virtual int? WATERBODY_COV_ { get; set; }

    [MappedField("WATERBODY_COV_ID")]
    public virtual int? WATERBODY_COV_ID { get; set; }

    [MappedField("HydroID")]
    public virtual int? HydroID { get; set; }

    [MappedField("HydroCode", 255)]
    public virtual string HydroCode { get; set; }

    [MappedField("FType", 255)]
    public virtual string FType { get; set; }

    [MappedField("Name", 255)]
    public virtual string Name { get; set; }

    [MappedField("AreaSqKm")]
    public virtual double? AreaSqKm { get; set; }

    [MappedField("JunctionID")]
    public virtual int? JunctionID { get; set; }
}
