using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Micro
{
    public class ServiceNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public ServiceNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetServiceName<T>()
        {
            return GetServiceName(typeof(T));
        }

        public static string GetServiceName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<ServiceNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
