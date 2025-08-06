namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Represents a produced message for test verification
    /// </summary>
    public class ProducedMessage
    {
        /// <summary>
        /// Gets or sets the message key
        /// </summary>
        public object? Key { get; set; }

        /// <summary>
        /// Gets or sets the message value
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the key type name
        /// </summary>
        public string KeyType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value type name
        /// </summary>
        public string ValueType { get; set; } = string.Empty;
    }
}