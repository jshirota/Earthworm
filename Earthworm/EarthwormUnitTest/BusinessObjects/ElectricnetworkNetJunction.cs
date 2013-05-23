using System;
using Earthworm;

public class ElectricnetworkNetJunction : MappableFeature
{
    [MappedField("Enabled")]
    public virtual short? Enabled { get; set; }

    [MappedField("County", 50)]
    public virtual string County { get; set; }
}
