using System;
using Earthworm;

public class CoarseCounty : MappableFeature
{

    [MappedField("NAME")]
    public virtual string Name { get; set; }

}

