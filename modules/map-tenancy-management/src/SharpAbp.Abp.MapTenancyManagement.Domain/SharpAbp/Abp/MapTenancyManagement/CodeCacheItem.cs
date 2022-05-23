namespace SharpAbp.Abp.MapTenancyManagement
{
    public class CodeCacheItem
    {
        public string Code { get; set; }
        public string MapCode { get; set; }

        public CodeCacheItem()
        {

        }

        public CodeCacheItem(string code, string mapCode)
        {
            Code = code;
            MapCode = mapCode;
        }
    }
}