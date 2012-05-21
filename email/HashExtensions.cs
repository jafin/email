using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using DotLiquid;

namespace email
{
    internal static class HashExtensions
    {
        private static readonly IDictionary<Type, PropertyInfo[]> _cachedStatics = new Dictionary<Type, PropertyInfo[]>();

        public static Hash FromDynamic(dynamic source)
        {
            var result = new Hash();
            if (source != null)
            {
                if (source is ExpandoObject)
                {
                    return Hash.FromDictionary((IDictionary<string, object>)source);
                }
                var type = (Type)source.GetType();
                if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
                {
                    throw new NotImplementedException("I don't feel like coding this up right now; use a static type?");
                }

                PropertyInfo[] properties;
                if (_cachedStatics.ContainsKey(type))
                {
                    properties = _cachedStatics[type];
                }
                else
                {
                    properties = type.GetProperties();
                    _cachedStatics.Add(type, properties);
                }
                foreach (var property in properties)
                {
                    result[property.Name] = property.GetValue(source, null);
                }
            }
            return result;
        }
    }
}