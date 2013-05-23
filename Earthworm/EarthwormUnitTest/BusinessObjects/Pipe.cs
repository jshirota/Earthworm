using System;
using Earthworm;

public class Pipe : MappableFeature
{
    [MappedField("Enabled")]
    public virtual short? Enabled { get; set; }
}
