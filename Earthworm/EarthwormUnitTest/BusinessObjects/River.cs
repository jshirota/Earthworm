using System;
using Earthworm;

public class River : MappableFeature
{
    [MappedField("NAME", 25)]
    public virtual string NAME { get; set; }

    [MappedField("GlobalID")]
    public virtual Guid GlobalID { get; private set; }
}
