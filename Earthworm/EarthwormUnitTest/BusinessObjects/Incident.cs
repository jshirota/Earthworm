using System;
using Earthworm;

public class Incident : MappableFeature
{
    [MappedField("req_id", 20)]
    public virtual string req_id { get; set; }

    [MappedField("req_type", 40)]
    public virtual string req_type { get; set; }

    [MappedField("req_date", 30)]
    public virtual string req_date { get; set; }

    [MappedField("req_time", 20)]
    public virtual string req_time { get; set; }

    [MappedField("address", 60)]
    public virtual string address { get; set; }

    [MappedField("x_coord", 20)]
    public virtual string x_coord { get; set; }

    [MappedField("y_coord", 20)]
    public virtual string y_coord { get; set; }

    [MappedField("district", 20)]
    public virtual string district { get; set; }

    [MappedField("status")]
    public virtual short? status { get; set; }
}
