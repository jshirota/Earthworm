using System;
using Earthworm;

public class Street : MappableFeature
{
    [MappedField("L_F_ADD", 12)]
    public virtual string L_F_ADD { get; set; }

    [MappedField("L_T_ADD", 12)]
    public virtual string L_T_ADD { get; set; }

    [MappedField("R_F_ADD", 12)]
    public virtual string R_F_ADD { get; set; }

    [MappedField("R_T_ADD", 12)]
    public virtual string R_T_ADD { get; set; }

    [MappedField("PREFIX", 8)]
    public virtual string PREFIX { get; set; }

    [MappedField("PRE_TYPE", 8)]
    public virtual string PRE_TYPE { get; set; }

    [MappedField("NAME", 32)]
    public virtual string NAME { get; set; }

    [MappedField("TYPE", 8)]
    public virtual string TYPE { get; set; }

    [MappedField("SUFFIX", 8)]
    public virtual string SUFFIX { get; set; }

    [MappedField("ZIPL", 8)]
    public virtual string ZIPL { get; set; }

    [MappedField("ZIPR", 8)]
    public virtual string ZIPR { get; set; }

    [MappedField("CITYL", 64)]
    public virtual string CITYL { get; set; }

    [MappedField("CITYR", 64)]
    public virtual string CITYR { get; set; }

    [MappedField("STATE_ABBR", 4)]
    public virtual string STATE_ABBR { get; set; }

    [MappedField("CFCC", 4)]
    public virtual string CFCC { get; set; }

    [MappedField("JoinID")]
    public virtual short? JoinID { get; set; }
}
