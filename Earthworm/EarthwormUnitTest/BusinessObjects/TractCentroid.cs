using System;
using Earthworm;

    public class TractCentroid : MappableFeature
    {
        [MappedField("ID", 25)]
        public virtual string ID { get; set; }

        [MappedField("NAME", 13)]
        public virtual string NAME { get; set; }

        [MappedField("STATE_NAME", 25)]
        public virtual string STATE_NAME { get; set; }

        [MappedField("AREA")]
        public virtual double? AREA { get; set; }

        [MappedField("POP2000")]
        public virtual int? POP2000 { get; set; }

        [MappedField("HOUSEHOLDS")]
        public virtual int? HOUSEHOLDS { get; set; }

        [MappedField("HSE_UNITS")]
        public virtual int? HSE_UNITS { get; set; }

        [MappedField("BUS_COUNT")]
        public virtual int? BUS_COUNT { get; set; }
    }
