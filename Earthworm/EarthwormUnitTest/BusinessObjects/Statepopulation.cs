using System;
using Earthworm;

public class Statepopulation : MappableFeature
{
    [MappedField("State_Name", 50)]
    public virtual string State_Name { get; set; }

    [MappedField("Start_Date")]
    public virtual DateTime? Start_Date { get; set; }

    [MappedField("Population")]
    public virtual double? Population { get; set; }
}
