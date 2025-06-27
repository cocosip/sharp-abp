using System;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringAbstractionsOptions
    {
        public int DefaultClientMaximumRetained { get; set; } = Environment.ProcessorCount * 2;
        public FileProviderConfigurations Providers { get; }
        public AbpFileStoringAbstractionsOptions()
        {
            Providers = new FileProviderConfigurations();
        }
    }
}
