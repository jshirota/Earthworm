namespace Earthworm.Serialization
{
    /// <summary>
    /// The JSON-serializable point type.
    /// </summary>
    public class JsonPoint : JsonGeometry, IJsonGeometry
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public double x { get; set; }

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public double y { get; set; }

        /// <summary>
        /// Deserializes a JSON string into an instance of this geometry type.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JsonPoint FromJson(string json)
        {
            return Json.Serializer.Deserialize<JsonPoint>(json);
        }
    }
}
