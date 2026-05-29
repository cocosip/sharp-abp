using System.Collections.Generic;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFilePathContextResolveOptions
    {
        public List<IFilePathContextResolveContributor> Contributors { get; }

        public AbpFilePathContextResolveOptions()
        {
            Contributors = [];
        }
    }
}
