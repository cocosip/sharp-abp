using AutoMapper;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementDomainModuleAutoMapperProfile : Profile
    {
        public FileStoringManagementDomainModuleAutoMapperProfile()
        {
            CreateMap<FileStoringContainer, FileStoringContainerEto>();
        }
    }
}
