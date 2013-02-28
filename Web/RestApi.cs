using System.Collections.Generic;

namespace Earthworm.Web
{
    internal class RestApi
    {
        public abstract class Response
        {
            public Error Error { get; set; }
        }

        public class FeatureCount : Response
        {
            public int? Count { get; set; }
        }

        public class FeatureSet : Response
        {
            public Dictionary<string, object>[] Features { get; set; }
        }

        public class LayerDefinition : Response
        {
            public string Type { get; set; }
            public string GeometryType { get; set; }
            public Extent Extent { get; set; }
            public Field[] Fields { get; set; }
        }

        public class Error
        {
            public string Description { get; set; }
            public string Message { get; set; }
        }

        public class Extent
        {
            public SpatialReference SpatialReference { get; set; }
        }

        public class Field
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class SpatialReference
        {
            public int Wkid { get; set; }
            public string Wkt { get; set; }
        }
    }
}
