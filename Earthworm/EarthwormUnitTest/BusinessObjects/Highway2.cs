using System;
using Earthworm;

public class Highway2 : MappableFeature
{
    [MappedField("LENGTH")]
    public virtual double? LENGTH { get; set; }

    [MappedField("TYPE", 40)]
    public virtual string TYPE { get; set; }

    [MappedField("ADMN_CLASS", 20)]
    public virtual string ADMN_CLASS { get; set; }

    [MappedField("TOLL_RD", 1)]
    public virtual string TOLL_RD { get; set; }

    [MappedField("RTE_NUM1", 3)]
    public virtual string RTE_NUM1 { get; set; }

    [MappedField("RTE_NUM2", 3)]
    public virtual string RTE_NUM2 { get; set; }

    [MappedField("ROUTE", 40)]
    public virtual string ROUTE { get; set; }
}
