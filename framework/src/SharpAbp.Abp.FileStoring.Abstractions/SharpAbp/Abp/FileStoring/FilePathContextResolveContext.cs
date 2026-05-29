using System;

namespace SharpAbp.Abp.FileStoring
{
    public class FilePathContextResolveContext : IFilePathContextResolveContext
    {
        public IServiceProvider ServiceProvider { get; }

        public FilePathContext? FilePathContext { get; set; }

        public bool Handled { get; set; }

        public FilePathContextResolveContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
