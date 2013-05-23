using System;
using Earthworm;

public class City : MappableFeature
{
    [MappedField("AREANAME")]
    public virtual string AREANAME { get; set; }

    [MappedField("CLASS")]
    public virtual string CLASS { get; set; }

    [MappedField("ST")]
    public virtual string ST { get; set; }

    [MappedField("CAPITAL")]
    public virtual string CAPITAL { get; set; }

    [MappedField("POP2000")]
    public virtual int? POP2000 { get; set; }

    public virtual string Province { get; set; }
}
