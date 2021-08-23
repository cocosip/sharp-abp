using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Core.Extensions
{
    public static class AbpDictionaryExtensions
    {

        /// <summary>
        /// Try add key,value to dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Remove value
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return dictionary.Remove(key);
            }
            return false;
        }

        /// <summary>
        /// Convert IEnumerable KeyValuePair as dictionary
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AsDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
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
        /// <typeparam name="DKey">Destination dictionary key type</typeparam>
        /// <typeparam name="DValue">Destination dictionary value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static IDictionary<DKey, DValue> ToDictionary<TKey, TValue, DKey, DValue>(
            this IDictionary<TKey, TValue> dictionary,
            Func<TKey, DKey> keySelector,
            Func<TValue, DValue> valueSelector)
        {
            var destination = new Dictionary<DKey, DValue>();
            foreach (var keyValuPair in dictionary)
            {
                destination.TryAdd(keySelector(keyValuPair.Key), valueSelector(keyValuPair.Value));
            }
            return destination;
        }

        /// <summary>
        /// Convert dictionary to new type dictionary
        /// </summary>
        /// <typeparam name="TKey">Source dictionary key type</typeparam>
        /// <typeparam name="TValue">Source dictionary value type</typeparam>
        /// <typeparam name="DValue">Destination dictionary value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static IDictionary<TKey, DValue> ToDictionary<TKey, TValue, DValue>(
            this IDictionary<TKey, TValue> dictionary,
            Func<TValue, DValue> valueSelector)
        {
            var destination = new Dictionary<TKey, DValue>();
            foreach (var keyValuePair in dictionary)
            {
                destination.TryAdd(keyValuePair.Key, valueSelector(keyValuePair.Value));
            }
            return destination;
        }


    }
}
