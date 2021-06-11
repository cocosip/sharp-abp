using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public SnowflakeNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetSnowflakeName<T>()
        {
            return GetSnowflakeName(typeof(T));
        }

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
