using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public class Office : MappableFeature
    {
        [MappedField("OFFICENAME", 0)]
        public virtual string OFFICENAME { get; set; }

        [MappedField("ADDRESS", 0)]
        public virtual string ADDRESS { get; set; }

        [MappedField("CITY", 0)]
        public virtual string CITY { get; set; }

        [MappedField("PROVINCE", 0)]
        public virtual string PROVINCE { get; set; }

        [MappedField("POSTALCODE", 0)]
        public virtual string POSTALCODE { get; set; }

        [MappedField("SUITEINFO", 0)]
        public virtual string SUITEINFO { get; set; }

        [MappedField("LAT")]
        public virtual double? LAT { get; set; }

        [MappedField("LONG_")]
        public virtual double? LONG_ { get; set; }

        [MappedField("PHONENUMBE", 0)]
        public virtual string PHONENUMBE { get; set; }

        [MappedField("EMAIL", 0)]
        public virtual string EMAIL { get; set; }

        [MappedField("NOMFRNCS", 0)]
        public virtual string NOMFRNCS { get; set; }

        [MappedField("Fax", 0)]
        public virtual string Fax { get; set; }

        [MappedField("X")]
        public virtual double? X { get; set; }

        [MappedField("Y")]
        public virtual double? Y { get; set; }

        [MappedField("Rm", 0)]
        public virtual string Rm { get; set; }

        [MappedField("ShortName", 0)]
        public virtual string ShortName { get; set; }

        [MappedField("Label", 0)]
        public virtual string Label { get; set; }

        [MappedField("IMAGEURL", 0)]
        public virtual string IMAGEURL { get; set; }

        [MappedField("OfficeType", 0)]
        public virtual string OfficeType { get; set; }
    }
}
