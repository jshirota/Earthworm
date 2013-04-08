namespace Earthworm.Serialization
{
    /// <summary>
    /// The JSON-serializable polygon type.
    /// </summary>
    public class JsonPolygon : JsonGeometry, IJsonGeometry
    {
        /// <summary>
        /// A jagged array representing points.
        /// </summary>
        public double[][][] rings { get; set; }

        /// <summary>
        /// Deserializes a JSON string into an instance of this geometry type.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JsonPolygon FromJson(string json)
        {
            return Json.Serializer.Deserialize<JsonPolygon>(json);
        }
    }
}
