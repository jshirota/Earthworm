using System;
using Earthworm;

public class Watershed : MappableFeature
{
    [MappedField("AREA")]
    public virtual double? AREA { get; set; }

    [MappedField("PERIMETER")]
    public virtual double? PERIMETER { get; set; }

    [MappedField("HUCREG12_")]
    public virtual int? HUCREG12_ { get; set; }

    [MappedField("HUCREG12_I")]
    public virtual int? HUCREG12_I { get; set; }

    [MappedField("HUC")]
    public virtual int? HUC { get; set; }

    [MappedField("REGION", 2)]
    public virtual string REGION { get; set; }

    [MappedField("SUBREGION", 2)]
    public virtual string SUBREGION { get; set; }

    [MappedField("ACCTUNIT", 2)]
    public virtual string ACCTUNIT { get; set; }

    [MappedField("HYDROUNIT", 2)]
    public virtual string HYDROUNIT { get; set; }

    [MappedField("OID_")]
    public virtual int? OID_ { get; set; }

    [MappedField("HUC_1")]
    public virtual int? HUC_1 { get; set; }

    [MappedField("NAME", 60)]
    public virtual string NAME { get; set; }

    [MappedField("HydroID")]
    public virtual int? HydroID { get; set; }

    [MappedField("HydroCode", 255)]
    public virtual string HydroCode { get; set; }

    [MappedField("DrainID")]
    public virtual int? DrainID { get; set; }

    [MappedField("AreaSqKm")]
    public virtual double? AreaSqKm { get; set; }

    [MappedField("JunctionID")]
    public virtual int? JunctionID { get; set; }

    [MappedField("NextDownID")]
    public virtual int? NextDownID { get; set; }
}
