using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Earthworm
{
    internal static class MappedPropertyExt
    {
        private static readonly ConcurrentDictionary<Type, List<MappedProperty>> TypeToMappedProperties = new ConcurrentDictionary<Type, List<MappedProperty>>();

        public static IEnumerable<MappedProperty> GetMappedProperties(this Type type)
        {
            return TypeToMappedProperties.GetOrAdd(type, t => type.GetProperties().Select(p => new MappedProperty(p)).Where(p => p.MappedField != null).ToList());
        }
    }
}
