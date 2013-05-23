using System;
using Earthworm;

    public class CompetitorStore : MappableFeature
    {
        [MappedField("NAME", 40)]
        public virtual string NAME { get; set; }
    }
