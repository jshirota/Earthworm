using System;
using Earthworm;

    public class FireStation : MappableFeature
    {
        [MappedField("Status", 1)]
        public virtual string Status { get; set; }

        [MappedField("Score")]
        public virtual short? Score { get; set; }

        [MappedField("Match_type", 2)]
        public virtual string Match_type { get; set; }

        [MappedField("Side", 1)]
        public virtual string Side { get; set; }

        [MappedField("Match_addr", 63)]
        public virtual string Match_addr { get; set; }

        [MappedField("NAME", 254)]
        public virtual string NAME { get; set; }

        [MappedField("ADDRESS", 254)]
        public virtual string ADDRESS { get; set; }
    }
