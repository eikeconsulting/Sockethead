using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sockethead.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Accumulate values for a key
        /// </summary>
        /// <param name="dictionary">base dictionary</param>
        /// <param name="key">Key to accumulate values upon</param>
        /// <param name="value">Value to add to the current accumulation</param>
        /// <param name="accumulator">method to take the current accumulation and the new value to include</param>
        public static Dictionary<TKey, TValue> Accumulate<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value,
            Func<TValue, TValue, TValue> accumulator)
        {
            dictionary[key] = dictionary.ContainsKey(key)
                ? accumulator(dictionary[key], value)
                : value;

            return dictionary;
        }

        /// <summary>
        /// Accumulate the sum into a key
        /// </summary>
        /// <remarks>There is no generic type for "add" so need specific overloads</remarks>
        public static Dictionary<TKey, decimal> AccumulateSum<TKey>(
            this Dictionary<TKey, decimal> dictionary,
            TKey key,
            decimal value)
        {
            return dictionary.Accumulate(key, value, (v1, v2) => v1 + v2);
        }

        /// <summary>
        /// Convert a dictionary of string keys and object values to an instance of a class.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }
    }
}