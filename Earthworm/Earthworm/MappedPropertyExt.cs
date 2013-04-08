using System;
using System.Collections.Generic;
using System.Reflection;

namespace Earthworm
{
    internal static class MappedPropertyExt
    {
        private static readonly Dictionary<Type, List<MappedProperty>> TypeToMappedProperties = new Dictionary<Type, List<MappedProperty>>();

        public static IEnumerable<MappedProperty> GetMappedProperties(this Type type)
        {
            if (TypeToMappedProperties.ContainsKey(type))
                return TypeToMappedProperties[type];

            List<MappedProperty> mappedProperties = new List<MappedProperty>();
            List<string> mappedPropertyNames = new List<string>();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (!mappedPropertyNames.Contains(p.Name))
                {
                    MappedProperty mappedProperty = new MappedProperty(p);

                    if (mappedProperty.MappedField != null)
                    {
                        mappedProperties.Add(mappedProperty);
                        mappedPropertyNames.Add(p.Name);
                    }
                }
            }

            TypeToMappedProperties.Add(type, mappedProperties);

            return mappedProperties;
        }
    }
}
