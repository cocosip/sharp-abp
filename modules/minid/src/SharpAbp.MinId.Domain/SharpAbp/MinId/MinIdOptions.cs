namespace SharpAbp.MinId
{
    public class MinIdOptions
    {
        /// <summary>
        /// Database execute concurrent retry count. (default:3) 
        /// </summary>
        public int ConflictRetryCount { get; set; } = 3;

        /// <summary>
        /// Load percent
        /// </summary>
        public int LoadingPercent { get; set; } = 20;

        /// <summary>
        /// LoadCurrentAsync,LoadNextAsync,timeout millis
        /// </summary>
        public int LoadSegmentIdTimeoutMillis { get; set; } = 10000;

        /// <summary>
        /// <see cref="DefaultMinIdGeneratorFactory.CreateAsync(string)"/>
        /// GetAsync timeout millis
        /// </summary>
        public int GetMinIdGeneratorTimeoutMillis { get; set; } = 10000;

        /// <summary>
        /// Timeout
        /// </summary>
        public int TransactionTimeout { get; set; } = 10000;
    }
}
