using System;
using Earthworm;

    public class DistributionCenter : MappableFeature
    {
        [MappedField("NAME", 50)]
        public virtual string NAME { get; set; }
    }
