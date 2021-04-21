using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Core.Extensions
{
    public static class AbpDictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }


        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.TryGetValue(key, out TValue o))
            {
                return o;
            }

            return defaultValue;
        }

        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return dictionary.Remove(key);
            }
            return false;
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var keyValuePair in keyValuePairs)
            {
                if (!dictionary.ContainsKey(keyValuePair.Key))
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Convert dictionary to new type dictionary
        /// </summary>
        /// <typeparam name="TKey">Source dictionary key type</typeparam>
        /// <typeparam name="TValue">Source dictionary value type</typeparam>
        /// <typeparam name="TTKey">Target dictionary key type</typeparam>
        /// <typeparam name="TTValue">Target dictionary value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static IDictionary<TTKey, TTValue> ConvertDictionary<TKey, TValue, TTKey, TTValue>(
            this IDictionary<TKey, TValue> dictionary,
            Func<TKey, TTKey> keySelector,
            Func<TValue, TTValue> valueSelector)
        {
            var dict = new Dictionary<TTKey, TTValue>();
            foreach (var keyValuPair in dictionary)
            {
                dict.TryAdd(keySelector(keyValuPair.Key), valueSelector(keyValuPair.Value));
            }
            return dict;
        }


    }
}
