using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public DbConnectionNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetDbConnectionName<T>()
        {
            return GetDbConnectionName(typeof(T));
        }

        public static string GetDbConnectionName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<DbConnectionNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}
