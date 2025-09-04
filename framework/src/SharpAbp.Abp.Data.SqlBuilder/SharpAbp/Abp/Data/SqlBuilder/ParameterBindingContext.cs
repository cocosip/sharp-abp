using System;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Represents the context for parameter binding operations in SQL parameter conversion.
    /// Contains information about the database provider, entity type, and input data.
    /// </summary>
    public class ParameterBindingContext
    {
        /// <summary>
        /// Gets the database provider type for the current operation.
        /// </summary>
        /// <value>The database provider enum value indicating the target database system</value>
        public DatabaseProvider DatabaseProvider { get; }
        
        /// <summary>
        /// Gets the entity type being processed for parameter conversion.
        /// </summary>
        /// <value>The Type of the entity that contains the data to be converted</value>
        public Type EntityType { get; }
        
        /// <summary>
        /// Gets the input data object that needs to be converted to database parameters.
        /// </summary>
        /// <value>The source object containing the data to be processed</value>
        public object? InputData { get; }

        /// <summary>
        /// Initializes a new instance of the ParameterBindingContext class
        /// </summary>
        public ParameterBindingContext()
        {
            EntityType = typeof(object);
            InputData = null;
        }

        /// <summary>
        /// Initializes a new instance of the ParameterBindingContext class with specified parameters
        /// </summary>
        /// <param name="databaseProvider">The database provider</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="inputData">The input data</param>
        public ParameterBindingContext(DatabaseProvider databaseProvider, Type entityType, object? inputData)
        {
            DatabaseProvider = databaseProvider;
            EntityType = entityType;
            InputData = inputData;
        }
    }
}