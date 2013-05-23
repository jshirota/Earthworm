﻿using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public class EvacuationPerimeter : MappableFeature
    {
        [MappedField("type")]
        public virtual int? type { get; set; }

        [MappedField("description", 1073741822)]
        public virtual string description { get; set; }
    }
}