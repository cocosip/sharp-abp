using SharpAbp.Abp.FileStoring;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class DatabaseFileContainerConfigurationProvider : IFileContainerConfigurationProvider
    {


        public FileContainerConfiguration Get(string name)
        {
            throw new NotImplementedException();
        }

        public List<FileContainerConfiguration> GetList(Func<FileContainerConfiguration, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
