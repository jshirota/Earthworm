using System;
using Earthworm;

public class Plantequip : MappableFeature
{
    [MappedField("PlantID")]
    public virtual short? PlantID { get; set; }

    [MappedField("EquipID")]
    public virtual short? EquipID { get; set; }

    [MappedField("EquipType", 50)]
    public virtual string EquipType { get; set; }
}
