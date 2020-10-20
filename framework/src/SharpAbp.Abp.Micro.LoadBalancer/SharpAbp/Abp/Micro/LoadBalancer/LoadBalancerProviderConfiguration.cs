using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerProviderConfiguration
    {
        public string Type { get; set; }

        private readonly Dictionary<string, Type> _properties;

        public LoadBalancerProviderConfiguration()
        {
            _properties = new Dictionary<string, Type>();
        }

        public LoadBalancerProviderConfiguration(string type)
        {
            Type = type;
            _properties = new Dictionary<string, Type>();
        }

        public Type GetProperty([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _properties.TryGetValue(name, out Type property);

            return property;
        }


        public LoadBalancerProviderConfiguration SetProperty([NotNull] string name, [NotNull] Type property)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(property, nameof(property));
            _properties.Add(name, property);
            return this;
        }

        public LoadBalancerProviderConfiguration ClearProperty([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _properties.Remove(name);
            return this;
        }

        public Dictionary<string, Type> GetProperties()
        {
            return _properties;
        }
    }
}
