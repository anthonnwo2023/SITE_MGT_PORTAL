﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Project.V1.Lib.Helpers
{
    public static class ExpandoMapperExtention
    {
        private static Dictionary<string, PropertyInfo> _propertyMap;

        static ExpandoMapperExtention()
        {
            // At this point we can convert each
            // property name to lower case so we avoid 
            // creating a new string more than once.
            //_propertyMap =
            //    typeof(T)
            //    .GetProperties()
            //    .ToDictionary(
            //        p => p.Name.ToLower(),
            //        p => p
            //    );
        }

        public static void Map<T>(this ExpandoObject source, T destination) where T : class
        {
            _propertyMap =
                typeof(T)
                .GetProperties()
                .ToDictionary(
                    p => p.Name,
                    p => p
                );

            // Might as well take care of null references early.
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.
            foreach (KeyValuePair<string, object> kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key, out p))
                {
                    Type propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (!propType.IsByRef && propType.Name != "Nullable`1")
                        {
                            // Throw if type is a value type 
                            // but not Nullable<>
                            throw new ArgumentException("not nullable");
                        }
                    }
                    else if (kv.Value.GetType() != propType)
                    {
                        // You could make this a bit less strict 
                        // but I don't recommend it.
                        throw new ArgumentException("type mismatch");
                    }
                    p.SetValue(destination, kv.Value, null);
                }
            }
        }
    }
}
