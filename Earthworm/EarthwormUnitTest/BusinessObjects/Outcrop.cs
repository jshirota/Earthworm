using System;
using Earthworm;

namespace EarthwormUnitTest
{


    public class Outcrop : MappableFeature
    {
        [MappedField("emp_id", 10)]
        public virtual string emp_id { get; set; }

        [MappedField("station_id", 10)]
        public virtual string station_id { get; set; }

        [MappedField("id_confidence", 25)]
        public virtual string id_confidence { get; set; }

        [MappedField("id_method", 25)]
        public virtual string id_method { get; set; }

        [MappedField("lithology_type", 25)]
        public virtual string lithology_type { get; set; }

        [MappedField("metamorphic_facies", 50)]
        public virtual string metamorphic_facies { get; set; }

        [MappedField("geomodifications", 25)]
        public virtual string geomodifications { get; set; }

        [MappedField("alteration", 50)]
        public virtual string alteration { get; set; }

        [MappedField("comments", 100)]
        public virtual string comments { get; set; }

        [MappedField("unit_id", 255)]
        public virtual string unit_id { get; set; }

    }

}
