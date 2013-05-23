using System;
using Earthworm;

public class Census : MappableFeature
{
    [MappedField("NAME", 32)]
    public virtual string NAME { get; set; }

    [MappedField("STATE_NAME", 25)]
    public virtual string STATE_NAME { get; set; }

    [MappedField("STATE_FIPS", 2)]
    public virtual string STATE_FIPS { get; set; }

    [MappedField("CNTY_FIPS", 3)]
    public virtual string CNTY_FIPS { get; set; }

    [MappedField("FIPS", 5)]
    public virtual string FIPS { get; set; }

    [MappedField("POP2000")]
    public virtual int? POP2000 { get; set; }

    [MappedField("POP2007")]
    public virtual int? POP2007 { get; set; }

    [MappedField("POP00_SQMI")]
    public virtual double? POP00_SQMI { get; set; }

    [MappedField("POP07_SQMI")]
    public virtual double? POP07_SQMI { get; set; }

    [MappedField("WHITE")]
    public virtual int? WHITE { get; set; }

    [MappedField("BLACK")]
    public virtual int? BLACK { get; set; }

    [MappedField("AMERI_ES")]
    public virtual int? AMERI_ES { get; set; }

    [MappedField("ASIAN")]
    public virtual int? ASIAN { get; set; }

    [MappedField("HAWN_PI")]
    public virtual int? HAWN_PI { get; set; }

    [MappedField("OTHER")]
    public virtual int? OTHER { get; set; }

    [MappedField("MULT_RACE")]
    public virtual int? MULT_RACE { get; set; }

    [MappedField("HISPANIC")]
    public virtual int? HISPANIC { get; set; }

    [MappedField("MALES")]
    public virtual int? MALES { get; set; }

    [MappedField("FEMALES")]
    public virtual int? FEMALES { get; set; }

    [MappedField("AGE_UNDER5")]
    public virtual int? AGE_UNDER5 { get; set; }

    [MappedField("AGE_5_17")]
    public virtual int? AGE_5_17 { get; set; }

    [MappedField("AGE_18_21")]
    public virtual int? AGE_18_21 { get; set; }

    [MappedField("AGE_22_29")]
    public virtual int? AGE_22_29 { get; set; }

    [MappedField("AGE_30_39")]
    public virtual int? AGE_30_39 { get; set; }

    [MappedField("AGE_40_49")]
    public virtual int? AGE_40_49 { get; set; }

    [MappedField("AGE_50_64")]
    public virtual int? AGE_50_64 { get; set; }

    [MappedField("AGE_65_UP")]
    public virtual int? AGE_65_UP { get; set; }

    [MappedField("MED_AGE")]
    public virtual double? MED_AGE { get; set; }

    [MappedField("MED_AGE_M")]
    public virtual double? MED_AGE_M { get; set; }

    [MappedField("MED_AGE_F")]
    public virtual double? MED_AGE_F { get; set; }

    [MappedField("HOUSEHOLDS")]
    public virtual int? HOUSEHOLDS { get; set; }

    [MappedField("AVE_HH_SZ")]
    public virtual double? AVE_HH_SZ { get; set; }

    [MappedField("HSEHLD_1_M")]
    public virtual int? HSEHLD_1_M { get; set; }

    [MappedField("HSEHLD_1_F")]
    public virtual int? HSEHLD_1_F { get; set; }

    [MappedField("MARHH_CHD")]
    public virtual int? MARHH_CHD { get; set; }

    [MappedField("MARHH_NO_C")]
    public virtual int? MARHH_NO_C { get; set; }

    [MappedField("MHH_CHILD")]
    public virtual int? MHH_CHILD { get; set; }

    [MappedField("FHH_CHILD")]
    public virtual int? FHH_CHILD { get; set; }

    [MappedField("FAMILIES")]
    public virtual int? FAMILIES { get; set; }

    [MappedField("AVE_FAM_SZ")]
    public virtual double? AVE_FAM_SZ { get; set; }

    [MappedField("HSE_UNITS")]
    public virtual int? HSE_UNITS { get; set; }

    [MappedField("VACANT")]
    public virtual int? VACANT { get; set; }

    [MappedField("OWNER_OCC")]
    public virtual int? OWNER_OCC { get; set; }

    [MappedField("RENTER_OCC")]
    public virtual int? RENTER_OCC { get; set; }

    [MappedField("NO_FARMS97")]
    public virtual double? NO_FARMS97 { get; set; }

    [MappedField("AVG_SIZE97")]
    public virtual double? AVG_SIZE97 { get; set; }

    [MappedField("CROP_ACR97")]
    public virtual double? CROP_ACR97 { get; set; }

    [MappedField("AVG_SALE97")]
    public virtual double? AVG_SALE97 { get; set; }

    [MappedField("SQMI")]
    public virtual double? SQMI { get; set; }
}
