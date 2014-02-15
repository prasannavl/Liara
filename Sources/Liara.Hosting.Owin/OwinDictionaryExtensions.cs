// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Collections.Generic;

namespace Liara.Extensions
{
    public static class OwinDictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;
            return dictionary.TryGetValue(key, out value) ? (T) value : default(T);
        }

        public static T Get<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            object value;
            return dictionary.TryGetValue(key, out value) ? (T) value : defaultValue;
        }

        public static T Get<T>(this IDictionary<string, object> dictionary, string subDictionaryKey, string key)
        {
            var subDictionary = dictionary.Get<IDictionary<string, object>>(subDictionaryKey);
            if (subDictionary == null)
            {
                return default(T);
            }

            return subDictionary.Get<T>(key);
        }

        public static T Get<T>(this IDictionary<string, object> dictionary, string subDictionaryKey, string key,
            T defaultValue)
        {
            var subDictionary = dictionary.Get<IDictionary<string, object>>(subDictionaryKey);
            if (subDictionary == null)
            {
                return defaultValue;
            }

            return subDictionary.Get(key, defaultValue);
        }

        public static void Set<T>(this IDictionary<string, object> dictionary, string key, T value)
        {
            dictionary[key] = value;
        }
    }
}