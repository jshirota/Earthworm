using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public class AtlasBase : MappableFeature
    {
        [MappedField("OBJECTID")]
        public virtual int? OBJECTID { get; set; }

        [MappedField("NAME", 0)]
        public virtual string NAME { get; set; }

        [MappedField("PREC_CODE", 0)]
        public virtual string PREC_CODE { get; set; }

        [MappedField("ATTRIBCODE", 0)]
        public virtual string ATTRIBCODE { get; set; }

        [MappedField("POI_ID", 0)]
        public virtual string POI_ID { get; set; }
    }

}
