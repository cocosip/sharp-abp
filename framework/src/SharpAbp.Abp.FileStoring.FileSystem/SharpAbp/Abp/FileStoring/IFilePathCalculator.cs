using System;
using System.Collections.Generic;
using System.Text;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFilePathCalculator
    {
        string Calculate(FileProviderArgs args);
    }
}
