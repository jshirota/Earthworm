using System;
using Earthworm;

    public class Hospital : MappableFeature
    {
        [MappedField("NAME", 50)]
        public virtual string NAME { get; set; }

        [MappedField("FCC", 3)]
        public virtual string FCC { get; set; }
    }
