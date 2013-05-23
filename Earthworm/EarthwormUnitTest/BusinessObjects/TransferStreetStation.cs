using System;
using Earthworm;

public class TransferStreetStation : MappableFeature
{
    [MappedField("Transittim")]
    public virtual double? Transittim { get; set; }
}
