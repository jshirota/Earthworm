using System;
using Earthworm;

    public class CentralDepot : MappableFeature
    {
        [MappedField("Name", 128)]
        public virtual string Name { get; set; }
    }
