using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// An attribute that specifies the name of a database connection
    /// </summary>
    public class DbConnectionNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the database connection
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the DbConnectionNameAttribute class with the specified name
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        public DbConnectionNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        /// <summary>
        /// Gets the name of the database connection for the specified type
        /// </summary>
        /// <param name="type">The type for which to get the database connection name</param>
        /// <returns>The name of the database connection</returns>
        public virtual string GetName(Type type)
        {
            return Name;
        }

        /// <summary>
        /// Gets the database connection name for the specified generic type
        /// </summary>
        /// <typeparam name="T">The type for which to get the database connection name</typeparam>
        /// <returns>The name of the database connection</returns>
        public static string GetDbConnectionName<T>()
        {
            return GetDbConnectionName(typeof(T));
        }

        /// <summary>
        /// Gets the database connection name for the specified type
        /// </summary>
        /// <param name="type">The type for which to get the database connection name</param>
        /// <returns>The name of the database connection</returns>
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