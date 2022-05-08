namespace SharpAbp.MinId
{
    public class MinIdInfoCacheItem
    {
        /// <summary>
        /// BizType
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// MaxId
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// Step
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Delta
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Remainder
        /// </summary>
        public int Remainder { get; set; }
    }
}
