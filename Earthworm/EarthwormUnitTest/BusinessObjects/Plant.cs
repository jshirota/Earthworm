using System;
using Earthworm;

public class Plant : MappableFeature
{
    [MappedField("PlantName", 50)]
    public virtual string PlantName { get; set; }

    [MappedField("PlantID")]
    public virtual short? PlantID { get; set; }

    [MappedField("Enabled")]
    public virtual short? Enabled { get; set; }
}
