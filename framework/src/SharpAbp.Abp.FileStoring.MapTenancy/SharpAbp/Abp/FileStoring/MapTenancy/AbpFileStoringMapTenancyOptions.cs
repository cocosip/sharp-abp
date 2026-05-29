namespace SharpAbp.Abp.FileStoring.MapTenancy
{
    public class AbpFileStoringMapTenancyOptions
    {
        public FilePathTenantCodeSource TenantCodeSource { get; set; }

        public MissingMapTenantBehavior MissingMapTenantBehavior { get; set; }

        public AbpFileStoringMapTenancyOptions()
        {
            TenantCodeSource = FilePathTenantCodeSource.Code;
            MissingMapTenantBehavior = MissingMapTenantBehavior.Ignore;
        }
    }
}
