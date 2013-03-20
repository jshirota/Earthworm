using System.Collections.Generic;

namespace Earthworm.Serialization
{
    internal class Graphic
    {
        public Dictionary<string, object> attributes { get; set; }
        public object geometry { get; set; }
    }
}
