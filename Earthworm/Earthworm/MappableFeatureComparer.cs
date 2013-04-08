using System.Collections.Generic;
using System.Linq;

namespace Earthworm
{
    /// <summary>
    /// Provides value equality comparison between two MappableFeature objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MappableFeatureComparer<T> : IEqualityComparer<T> where T : MappableFeature, new()
    {
        /// <summary>
        /// Returns true if two objects have the same attribute values and geometry.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return x.ValueEquals(y);
        }

        /// <summary>
        /// Returns the hash code of the concatenated attribute values.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return string.Join("\t", obj.ToKeyValuePairs().Select(o => (o.Value ?? "").ToString()).ToArray()).GetHashCode();
        }
    }
}
