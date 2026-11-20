using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public FileContainerNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetContainerName<T>()
        {
            return GetContainerName(typeof(T));
        }

        public static string GetContainerName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<FileContainerNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName ?? throw new ArgumentException("The container type must have a full name.", nameof(type));
            }

            return nameAttribute.GetName(type);
        }
    }
}
