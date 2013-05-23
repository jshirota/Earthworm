using System;
using Earthworm;

    public class Route : MappableFeature
    {
        [MappedField("Name", 128)]
        public virtual string Name { get; set; }

        [MappedField("Description", 128)]
        public virtual string Description { get; set; }

        [MappedField("StartDepotName", 128)]
        public virtual string StartDepotName { get; set; }

        [MappedField("EndDepotName", 128)]
        public virtual string EndDepotName { get; set; }

        [MappedField("StartDepotServiceTime")]
        public virtual double? StartDepotServiceTime { get; set; }

        [MappedField("EndDepotServiceTime")]
        public virtual double? EndDepotServiceTime { get; set; }

        [MappedField("EarliestStartTime")]
        public virtual DateTime EarliestStartTime { get; set; }

        [MappedField("LatestStartTime")]
        public virtual DateTime LatestStartTime { get; set; }

        [MappedField("Capacities", 128)]
        public virtual string Capacities { get; set; }

        [MappedField("FixedCost")]
        public virtual double? FixedCost { get; set; }

        [MappedField("CostPerUnitTime")]
        public virtual double CostPerUnitTime { get; set; }

        [MappedField("CostPerUnitDistance")]
        public virtual double? CostPerUnitDistance { get; set; }

        [MappedField("OvertimeStartTime")]
        public virtual double? OvertimeStartTime { get; set; }

        [MappedField("CostPerUnitOvertime")]
        public virtual double? CostPerUnitOvertime { get; set; }

        [MappedField("MaxOrderCount")]
        public virtual int MaxOrderCount { get; set; }

        [MappedField("MaxTotalTime")]
        public virtual double? MaxTotalTime { get; set; }

        [MappedField("MaxTotalTravelTime")]
        public virtual double? MaxTotalTravelTime { get; set; }

        [MappedField("MaxTotalDistance")]
        public virtual double? MaxTotalDistance { get; set; }

        [MappedField("SpecialtyNames", 128)]
        public virtual string SpecialtyNames { get; set; }

        [MappedField("AssignmentRule")]
        public virtual int AssignmentRule { get; set; }
    }
