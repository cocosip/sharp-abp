namespace SharpAbp.Abp.MassTransit.TestImplementations
{
    /// <summary>
    /// Represents a sent message for test verification
    /// </summary>
    public class SentMessage
    {
        /// <summary>
        /// Gets or sets the destination URI string
        /// </summary>
        public string UriString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message object
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// Gets or sets the message type name
        /// </summary>
        public string MessageType { get; set; } = string.Empty;
    }
}