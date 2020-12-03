using AutoMapper;
using SharpAbp.Abp.FileStoring;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementApplicationModuleAutoMapperProfile: Profile
    {
        public FileStoringManagementApplicationModuleAutoMapperProfile()
        {
            CreateMap<FileProviderConfiguration, ProviderDto>();
        }
    }
}
