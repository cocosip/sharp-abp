namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Represents a published message for test verification
    /// </summary>
    public class PublishedMessage
    {
        /// <summary>
        /// Gets or sets the message object
        /// </summary>
        public object? Message { get; set; }

        /// <summary>
        /// Gets or sets the message type name
        /// </summary>
        public string MessageType { get; set; } = string.Empty;
    }
}