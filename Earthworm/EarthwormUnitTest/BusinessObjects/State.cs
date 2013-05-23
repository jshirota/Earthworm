using System;
using Earthworm;

public class State : MappableFeature
{
    [MappedField("AREA")]
    public virtual double? AREA { get; set; }

    [MappedField("STATE_NAME")]
    public virtual string STATE_NAME { get; set; }

    [MappedField("SUB_REGION")]
    public virtual string SUB_REGION { get; set; }

    [MappedField("STATE_ABBR")]
    public virtual string STATE_ABBR { get; set; }

    [MappedField("POP2000")]
    public virtual int? POP2000 { get; set; }

    [MappedField("POP00_SQMI")]
    public virtual int? POP00_SQMI { get; set; }
}
