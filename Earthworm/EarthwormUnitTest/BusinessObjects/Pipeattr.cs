using System;
using Earthworm;

public class Pipeattr : MappableFeature
{
    [MappedField("Diameter")]
    public virtual short? Diameter { get; set; }
}
