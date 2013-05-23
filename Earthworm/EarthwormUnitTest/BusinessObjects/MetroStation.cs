using System;
using Earthworm;

public class MetroStation : MappableFeature
{
    [MappedField("ID_STATION")]
    public virtual int? ID_STATION { get; set; }

    [MappedField("ID_LINE")]
    public virtual int? ID_LINE { get; set; }

    [MappedField("NAME", 50)]
    public virtual string NAME { get; set; }

    [MappedField("StopTime")]
    public virtual double? StopTime { get; set; }
}
