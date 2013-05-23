using System;
using Earthworm;

public class TransferStation : MappableFeature
{
    [MappedField("NAME", 120)]
    public virtual string NAME { get; set; }

    [MappedField("Transittim")]
    public virtual double? Transittim { get; set; }
}
