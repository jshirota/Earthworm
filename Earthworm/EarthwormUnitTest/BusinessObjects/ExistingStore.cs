using System;
using Earthworm;

    public class ExistingStore : MappableFeature
    {
        [MappedField("NAME", 50)]
        public virtual string NAME { get; set; }
    }
