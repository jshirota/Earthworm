using System;
using Earthworm;

public class Customer : MappableFeature
{
    [MappedField("NAME", 29)]
    public virtual string NAME { get; set; }

    [MappedField("ADDRESS", 50)]
    public virtual string ADDRESS { get; set; }

    [MappedField("ZIP", 5)]
    public virtual string ZIP { get; set; }

    [MappedField("TYPE", 20)]
    public virtual string TYPE { get; set; }

    [MappedField("SALES")]
    public virtual float? SALES { get; set; }
}
