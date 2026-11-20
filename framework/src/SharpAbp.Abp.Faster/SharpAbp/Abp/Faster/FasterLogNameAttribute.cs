using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.Faster
{
    public class FasterLogNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public FasterLogNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetLogName<T>()
        {
            return GetLogName(typeof(T));
        }

        public static string GetLogName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<FasterLogNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName ?? throw new ArgumentException("The type must have a full name or an explicit Faster log name.", nameof(type));
            }

            return nameAttribute.GetName(type);
        }

    }
}
