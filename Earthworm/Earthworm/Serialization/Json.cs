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
            var attributes = new Dictionary<string, object>();

            if (feature.IsDataBound)
                attributes.Add(feature.Table.OIDFieldName, feature.OID);

            foreach (var o in feature.ToKeyValuePairs(p => p.MappedField.IncludeInJson))
            {
                var value = o.Value;

                if (value != null)
                {
                    if (value is DateTime)
                        value = Convert.ToInt64(((DateTime)value).Subtract(BaseTime).TotalMilliseconds);
                    else if (value is Guid)
                        value = ((Guid)value).ToString("B").ToUpper();
                }

                attributes.Add(o.Key, value);
            }

            var dictionary = new Dictionary<string, object> { { "attributes", attributes } };

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
            var mappableFeature = new T();

            var graphic = Serializer.Deserialize<Graphic>(json);

            foreach (var property in typeof(T).GetMappedProperties())
            {
                if (!graphic.attributes.ContainsKey(property.MappedField.FieldName))
                    continue;

                var value = graphic.attributes[property.MappedField.FieldName];

                if (value != null && (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)))
                    value = BaseTime.AddMilliseconds((long)value);

                property.SetValue(mappableFeature, value, true);
            }

            var dictionary = graphic.geometry as Dictionary<string, object>;

            if (dictionary != null)
            {
                json = Serializer.Serialize(dictionary);

                IJsonGeometry geometry;

                string[] keys = { "points", "paths", "rings" };
                var key = keys.SingleOrDefault(dictionary.ContainsKey);

                if (key == null && dictionary.ContainsKey("x") && dictionary.ContainsKey("y"))
                    geometry = Serializer.Deserialize<JsonPoint>(json);
                else switch (key)
                    {
                        case "points":
                            geometry = Serializer.Deserialize<JsonMultipoint>(json);
                            break;
                        case "paths":
                            geometry = Serializer.Deserialize<JsonPolyline>(json);
                            break;
                        case "rings":
                            geometry = Serializer.Deserialize<JsonPolygon>(json);
                            break;
                        default:
                            throw new Exception("Invalid geometry object.");
                    }

                mappableFeature.Shape = geometry.ToEsriGeometry();
            }

            return mappableFeature;
        }
    }
}
