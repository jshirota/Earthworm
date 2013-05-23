using System;
using Earthworm;

public class MyfeaturedatasetNetJunction : MappableFeature
{
    [MappedField("Enabled")]
    public virtual short? Enabled { get; set; }
}
