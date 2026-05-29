using System;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFilePathContextResolveContext
    {
        IServiceProvider ServiceProvider { get; }

        FilePathContext? FilePathContext { get; set; }

        bool Handled { get; set; }
    }
}
