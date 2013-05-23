using System;
using Earthworm;

public class PlaceAliase : MappableFeature
{
    [MappedField("NAME", 29)]
    public virtual string NAME { get; set; }

    [MappedField("ADDRESS", 24)]
    public virtual string ADDRESS { get; set; }

    [MappedField("ZIP", 5)]
    public virtual string ZIP { get; set; }
}
