using System;
using Earthworm;

public class Servicelocation : MappableFeature
{
    [MappedField("LOCATIONID", 30)]
    public virtual string LOCATIONID { get; set; }

    [MappedField("PHASECODE")]
    public virtual int? PHASECODE { get; set; }

    [MappedField("PIM", 40)]
    public virtual string PIM { get; set; }

    [MappedField("ONWHAT", 10)]
    public virtual string ONWHAT { get; set; }

    [MappedField("County", 50)]
    public virtual string County { get; set; }

    [MappedField("TRANSFACILITYID", 20)]
    public virtual string TRANSFACILITYID { get; set; }

    [MappedField("SERVICETYPE", 50)]
    public virtual string SERVICETYPE { get; set; }

    [MappedField("SERIALNUMBER", 20)]
    public virtual string SERIALNUMBER { get; set; }

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

    [MappedField("TRANSFORMERUID", 40)]
    public virtual string TRANSFORMERUID { get; set; }

    [MappedField("IDLE", 3)]
    public virtual string IDLE { get; set; }

    [MappedField("X")]
    public virtual double? X { get; set; }

    [MappedField("Y")]
    public virtual double? Y { get; set; }

    [MappedField("Z")]
    public virtual double? Z { get; set; }

    [MappedField("Name", 50)]
    public virtual string Name { get; set; }

    [MappedField("Attn", 50)]
    public virtual string Attn { get; set; }

    [MappedField("ServiceAddress", 50)]
    public virtual string ServiceAddress { get; set; }

    [MappedField("BillingAddress", 50)]
    public virtual string BillingAddress { get; set; }

    [MappedField("City", 50)]
    public virtual string City { get; set; }

    [MappedField("State", 2)]
    public virtual string State { get; set; }

    [MappedField("Zip", 50)]
    public virtual string Zip { get; set; }

    [MappedField("Meter", 50)]
    public virtual string Meter { get; set; }

    [MappedField("Phone", 50)]
    public virtual string Phone { get; set; }

    [MappedField("ServiceVoltage", 10)]
    public virtual string ServiceVoltage { get; set; }

    [MappedField("BoardDist", 5)]
    public virtual string BoardDist { get; set; }

    [MappedField("TaxDist", 5)]
    public virtual string TaxDist { get; set; }

    [MappedField("FranchiseDist", 5)]
    public virtual string FranchiseDist { get; set; }

    [MappedField("SchoolDist", 5)]
    public virtual string SchoolDist { get; set; }

    [MappedField("District", 5)]
    public virtual string District { get; set; }

    [MappedField("Substation", 4)]
    public virtual string Substation { get; set; }

    [MappedField("Feeder", 5)]
    public virtual string Feeder { get; set; }

    [MappedField("EaLoc", 15)]
    public virtual string EaLoc { get; set; }

    [MappedField("SurveySection")]
    public virtual int? SurveySection { get; set; }

    [MappedField("SurveyTownship", 4)]
    public virtual string SurveyTownship { get; set; }

    [MappedField("SurveyRange", 4)]
    public virtual string SurveyRange { get; set; }
}
