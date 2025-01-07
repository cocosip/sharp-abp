namespace SharpAbp.Abp.Faster
{
    public class AbpFasterOptions
    {
        /// <summary>
        /// 文件存储的根目录
        /// </summary>
        public string RootPath { get; set; }
        public AbpFasterConfigurations Configurations { get; }
        public AbpFasterOptions()
        {
            Configurations = new AbpFasterConfigurations();
        }
    }
}
