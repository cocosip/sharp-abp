namespace SharpAbp.MinId
{
    public static class MinIdInfoExtensions
    {

        public static MinIdInfoCacheItem ToCacheItem(this MinIdInfo minIdInfo)
        {
            return new MinIdInfoCacheItem()
            {
                BizType = minIdInfo.BizType,
                MaxId = minIdInfo.MaxId,
                Step = minIdInfo.Step,
                Delta = minIdInfo.Delta,
                Remainder = minIdInfo.Remainder
            };
        }
    }
}
