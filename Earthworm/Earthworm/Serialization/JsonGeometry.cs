namespace Earthworm.Serialization
{
    /// <summary>
    /// The base type for all JSON-serializable object types.
    /// </summary>
    public abstract class JsonGeometry
    {
        /// <summary>
        /// Return the JSON representation of this geometry.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Json.Serializer.Serialize(this);
        }
    }
}
