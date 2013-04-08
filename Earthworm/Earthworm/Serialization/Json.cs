using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Earthworm.Serialization
{
    /// <summary>
    /// Provides extension methods for converting features into JSON strings.
    /// </summary>
    public static class Json
    {
        internal static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer { MaxJsonLength = 67108864 };
        internal static readonly DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// The function used to serialize an object into a JSON string.  By default, this is set to System.Web.Script.Serialization.JavaScriptSerializer.Serialize.  This can be replaced by another function such as o => Newtonsoft.Json.JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented).
        /// </summary>
        public static Func<object, string> SerializingFunction { get; set; }

        static Json()
        {
            SerializingFunction = o => Serializer.Serialize(o);
        }

        /// <summary>
        /// Returns the JSON-serializable representation of this feature as a .NET dictionary.  Use a weakly-typed JSON serializer to serialize this.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="includeGeometry"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this MappableFeature feature, bool includeGeometry = false)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            if (feature.IsDataBound)
                attributes.Add(feature.Table.OIDFieldName, feature.OID);

            foreach (KeyValuePair<string, object> o in feature.ToKeyValuePairs(p => p.MappedField.IncludeInJson))
            {
                object value = o.Value;

                if (value != null)
                {
                    if (value is DateTime)
                        value = Convert.ToInt64(((DateTime)value).Subtract(BaseTime).TotalMilliseconds);
                    else if (value is Guid)
                        value = ((Guid)value).ToString("B").ToUpper();
                }

                attributes.Add(o.Key, value);
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("attributes", attributes);

            if (includeGeometry && feature.Shape != null)
                dictionary.Add("geometry", feature.Shape.ToJsonGeometry());

            return dictionary;
        }

        /// <summary>
        /// Return the JSON representation of this feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="includeGeometry"></param>
        /// <returns></returns>
        public static string Serialize(MappableFeature feature, bool includeGeometry)
        {
            return SerializingFunction(feature.ToDictionary(includeGeometry));
        }

        /// <summary>
        /// Creates a new feature from a JSON string.  OID is ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json) where T : MappableFeature, new()
        {
            T mappableFeature = new T();

            Graphic graphic = Serializer.Deserialize<Graphic>(json);

            foreach (MappedProperty property in typeof(T).GetMappedProperties())
            {
                if (graphic.attributes.ContainsKey(property.MappedField.FieldName))
                {
                    object value = graphic.attributes[property.MappedField.FieldName];

                    if (value != null && (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)))
                        value = BaseTime.AddMilliseconds((long)value);

                    property.SetValue(mappableFeature, value, true);
                }
            }

            Dictionary<string, object> dictionary = graphic.geometry as Dictionary<string, object>;

            if (dictionary != null)
            {
                json = Serializer.Serialize(dictionary);

                IJsonGeometry geometry;

                string[] keys = { "points", "paths", "rings" };
                string key = keys.SingleOrDefault(dictionary.ContainsKey);

                if (key == null && dictionary.ContainsKey("x") && dictionary.ContainsKey("y"))
                    geometry = Serializer.Deserialize<JsonPoint>(json);
                else if (key == "points")
                    geometry = Serializer.Deserialize<JsonMultipoint>(json);
                else if (key == "paths")
                    geometry = Serializer.Deserialize<JsonPolyline>(json);
                else if (key == "rings")
                    geometry = Serializer.Deserialize<JsonPolygon>(json);
                else
                    throw new Exception("Invalid geometry object.");

                mappableFeature.Shape = geometry.ToEsriGeometry();
            }

            return mappableFeature;
        }
    }
}
