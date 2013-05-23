using System;
using Earthworm;

public class Highway : MappableFeature
{
    [MappedField("LENGTH")]
    public virtual double? LENGTH { get; set; }

    [MappedField("TYPE")]
    public virtual string TYPE { get; set; }

    [MappedField("ADMN_CLASS")]
    public virtual string ADMN_CLASS { get; set; }

    [MappedField("TOLL_RD")]
    public virtual string TOLL_RD { get; set; }

    [MappedField("RTE_NUM1")]
    public virtual string RTE_NUM1 { get; set; }

    [MappedField("RTE_NUM2")]
    public virtual string RTE_NUM2 { get; set; }

    [MappedField("ROUTE")]
    public virtual string ROUTE { get; set; }
}
