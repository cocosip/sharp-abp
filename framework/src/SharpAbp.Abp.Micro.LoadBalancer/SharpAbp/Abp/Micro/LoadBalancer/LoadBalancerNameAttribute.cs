using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public LoadBalancerNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetLoadBalancerName<T>()
        {
            return GetLoadBalancerName(typeof(T));
        }

        public static string GetLoadBalancerName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<LoadBalancerNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
