using System;
using Earthworm;

public class City2 : City
{
    [MappedField("Comment")]
    public virtual string Comment { get; set; }
}
