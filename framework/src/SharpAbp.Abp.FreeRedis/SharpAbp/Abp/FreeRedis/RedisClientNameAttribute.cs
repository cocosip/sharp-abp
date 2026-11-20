using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.FreeRedis
{
    public class RedisClientNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public RedisClientNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetClientName<T>()
        {
            return GetClientName(typeof(T));
        }

        public static string GetClientName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<RedisClientNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName ?? throw new ArgumentException("The type must have a full name or an explicit Redis client name.", nameof(type));
            }

            return nameAttribute.GetName(type);
        }

    }
}
