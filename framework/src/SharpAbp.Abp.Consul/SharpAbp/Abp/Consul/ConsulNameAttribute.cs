using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Consul
{
    public class ConsulNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public ConsulNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetConsulName<T>()
        {
            return GetConsulName(typeof(T));
        }

        public static string GetConsulName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<ConsulNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
