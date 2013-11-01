using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Earthworm
{
    internal static class MappedPropertyExt
    {
        private static readonly ConcurrentDictionary<Type, List<MappedProperty>> TypeToMappedProperties = new ConcurrentDictionary<Type, List<MappedProperty>>();

        public static IEnumerable<MappedProperty> GetMappedProperties(this Type type)
        {
            return TypeToMappedProperties.GetOrAdd(type, t =>
            {
                var mappedProperties = new List<MappedProperty>();
                var mappedPropertyNames = new List<string>();

                foreach (var p in type.GetProperties())
                {
                    if (!mappedPropertyNames.Contains(p.Name))
                    {
                        var mappedProperty = new MappedProperty(p);

                        if (mappedProperty.MappedField != null)
                        {
                            mappedProperties.Add(mappedProperty);
                            mappedPropertyNames.Add(p.Name);
                        }
                    }
                }

                return mappedProperties;
            });
        }
    }
}
