using System;
using Earthworm;

public class UsaMajorHighway : MappableFeature
{
    [MappedField("NAME", 5)]
    public virtual string NAME { get; set; }

    [MappedField("FULL_NAME", 8)]
    public virtual string FULL_NAME { get; set; }

    [MappedField("Shape_Leng")]
    public virtual double? Shape_Leng { get; set; }

    [MappedField("RuleID")]
    public virtual int? RuleID { get; set; }

    [MappedField("Override")]
    public virtual byte[] Override { get; set; }
}
