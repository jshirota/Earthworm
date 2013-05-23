using System;
using Earthworm;

public class CandidateStore : MappableFeature
{
    [MappedField("NAME", 45)]
    public virtual string NAME { get; set; }
}
