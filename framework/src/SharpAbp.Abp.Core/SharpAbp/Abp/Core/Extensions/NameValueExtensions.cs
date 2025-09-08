using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.Core.Extensions
{
    /// <summary>
    /// Extension methods for NameValue collections providing search and value retrieval functionality
    /// </summary>
    public static class NameValueExtensions
    {
        /// <summary>
        /// Finds the first NameValue item with the specified name, or returns null if not found
        /// </summary>
        /// <param name="values">The collection of NameValue items to search</param>
        /// <param name="name">The name to search for</param>
        /// <returns>The first matching NameValue item, or null if not found</returns>
        public static NameValue FindOrDefault(this IList<NameValue> values, string name)
        {
            return values.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Finds the value of the first NameValue item with the specified name
        /// </summary>
        /// <param name="values">The collection of NameValue items to search</param>
        /// <param name="name">The name to search for</param>
        /// <returns>The value of the first matching NameValue item, or empty string if not found</returns>
        public static string FindValue(this IList<NameValue> values, string name)
        {
            return values.FirstOrDefault(x => x.Name == name)?.Value ?? "";
        }
    }
}
