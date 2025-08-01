using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// An attribute used to define a custom name for a Snowflake instance.
    /// This allows for more descriptive and user-friendly identification of Snowflake configurations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SnowflakeNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the Snowflake instance.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the Snowflake instance. Must not be null or whitespace.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public SnowflakeNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        /// <summary>
        /// Gets the name of the Snowflake instance for a given type.
        /// </summary>
        /// <param name="type">The type to get the Snowflake name for.</param>
        /// <returns>The name of the Snowflake instance.</returns>
        public virtual string GetName(Type type)
        {
            return Name;
        }

        /// <summary>
        /// Statically gets the Snowflake name for a given generic type.
        /// </summary>
        /// <typeparam name="T">The type to get the Snowflake name for.</typeparam>
        /// <returns>The name of the Snowflake instance.</returns>
        public static string GetSnowflakeName<T>()
        {
            return GetSnowflakeName(typeof(T));
        }

        /// <summary>
        /// Statically gets the Snowflake name for a given type.
        /// If the <see cref="SnowflakeNameAttribute"/> is not found on the type, the full name of the type is returned.
        /// </summary>
        /// <param name="type">The type to get the Snowflake name for.</param>
        /// <returns>The name of the Snowflake instance.</returns>
        public static string GetSnowflakeName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<SnowflakeNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
