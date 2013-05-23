using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public class Plan : MappableFeature
    {
        [MappedField("UserID")]
        public virtual int? UserID { get; set; }

        [MappedField("BaseFeatureClass", 50)]
        public virtual string BaseFeatureClass { get; set; }

        [MappedField("Title", 50)]
        public virtual string Title { get; set; }

        [MappedField("Description", 500)]
        public virtual string Description { get; set; }

        [MappedField("DateCreated")]
        public virtual DateTime? DateCreated { get; set; }

        [MappedField("NaturalLandscapeGoal")]
        public virtual short? NaturalLandscapeGoal { get; set; }

        [MappedField("MammalHabitatGoal")]
        public virtual short? MammalHabitatGoal { get; set; }

        [MappedField("FishHealthGoal")]
        public virtual short? FishHealthGoal { get; set; }

        [MappedField("WaterQualityGoal")]
        public virtual short? WaterQualityGoal { get; set; }

        [MappedField("BioticCarbonGoal")]
        public virtual short? BioticCarbonGoal { get; set; }

        [MappedField("GreenhouseGoal")]
        public virtual short? GreenhouseGoal { get; set; }

        [MappedField("HumanPopGoal")]
        public virtual short? HumanPopGoal { get; set; }

        [MappedField("GDPGoal")]
        public virtual short? GDPGoal { get; set; }

        [MappedField("HydrocarbonGoal")]
        public virtual short? HydrocarbonGoal { get; set; }

        [MappedField("TimberGoal")]
        public virtual short? TimberGoal { get; set; }

        [MappedField("AgricultureGoal")]
        public virtual short? AgricultureGoal { get; set; }

        [MappedField("WaterConsumptionGoal")]
        public virtual short? WaterConsumptionGoal { get; set; }
    }
}
