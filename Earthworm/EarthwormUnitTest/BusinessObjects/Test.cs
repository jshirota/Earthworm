using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public  class Test : MappableFeature
    {
        [MappedField("PERIMETER")]
        public virtual double? PERIMETER { get; set; }

        [MappedField("PGAPOL_")]
        public virtual double? PGAPOL_ { get; set; }

        [MappedField("PGAPOL_ID")]
        public virtual double? PGAPOL_ID { get; set; }

        [MappedField("GRID_CODE")]
        public virtual double? GRID_CODE { get; set; }

        [MappedField("VALUE")]
        public virtual double? VALUE { get; set; }

        [MappedField("EVENTGUID", 255)]
        public virtual string EVENTGUID { get; set; }

        [MappedField("EVENTID", 255)]
        public virtual string EVENTID { get; set; }

        [MappedField("EVENTREGION", 255)]
        public virtual string EVENTREGION { get; set; }

        [MappedField("EVENTDATE")]
        public virtual DateTime? EVENTDATE { get; set; }

        [MappedField("EVENTMAGNITUDE")]
        public virtual double? EVENTMAGNITUDE { get; set; }

        [MappedField("INFO_LINK", 255)]
        public virtual string INFO_LINK { get; set; }

        [MappedField("LOCATION", 255)]
        public virtual string LOCATION { get; set; }
    }
}
