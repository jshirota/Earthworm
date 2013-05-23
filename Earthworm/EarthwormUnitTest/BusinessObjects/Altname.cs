using System;
using Earthworm;

public class Altname : MappableFeature
{
    [MappedField("JOINID")]
    public virtual int? JOINID { get; set; }

    [MappedField("PRE_DIR", 4)]
    public virtual string PRE_DIR { get; set; }

    [MappedField("PRE_TYPE", 8)]
    public virtual string PRE_TYPE { get; set; }

    [MappedField("ST_NAME", 40)]
    public virtual string ST_NAME { get; set; }

    [MappedField("ST_TYPE", 8)]
    public virtual string ST_TYPE { get; set; }

    [MappedField("SUF_DIR", 6)]
    public virtual string SUF_DIR { get; set; }
}
