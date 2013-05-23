using System;
using Earthworm;

    public class Store : MappableFeature
    {
        [MappedField("NAME", 45)]
        public virtual string NAME { get; set; }

        [MappedField("Demand")]
        public virtual double? Demand { get; set; }

        [MappedField("ServiceTime")]
        public virtual double? ServiceTime { get; set; }

        [MappedField("TimeStart1")]
        public virtual DateTime? TimeStart1 { get; set; }

        [MappedField("TimeEnd1")]
        public virtual DateTime? TimeEnd1 { get; set; }
    }
