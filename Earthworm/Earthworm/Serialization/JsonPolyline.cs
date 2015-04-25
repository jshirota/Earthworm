namespace Earthworm.Serialization
{
    /// <summary>
    /// The JSON-serializable polyline type.
    /// </summary>
    public class JsonPolyline : JsonGeometry, IJsonGeometry
    {
        /// <summary>
        /// A jagged array representing points.
        /// </summary>
        public double[][][] paths { get; set; }

        /// <summary>
        /// Deserializes a JSON string into an instance of this geometry type.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JsonPolyline FromJson(string json)
        {
            return Json.Serializer.Deserialize<JsonPolyline>(json);
        }
    }
}
