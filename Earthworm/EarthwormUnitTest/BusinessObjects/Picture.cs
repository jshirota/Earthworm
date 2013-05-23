using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Earthworm;

public class Picture : MappableFeature
{
    [MappedField("Name")]
    public virtual string Name { get; set; }

    [MappedField("Data")]
    public virtual byte[] Data { get; set; }

}
