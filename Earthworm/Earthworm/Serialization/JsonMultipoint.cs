namespace Earthworm.Serialization
{
    /// <summary>
    /// The JSON-serializable multipoint type.
    /// </summary>
    public class JsonMultipoint : JsonGeometry, IJsonGeometry
    {
        /// <summary>
        /// A jagged array representing points.
        /// </summary>
        public double[][] points { get; set; }

        /// <summary>
        /// Deserializes a JSON string into an instance of this geometry type.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JsonMultipoint FromJson(string json)
        {
            return Json.Serializer.Deserialize<JsonMultipoint>(json);
        }
    }
}
