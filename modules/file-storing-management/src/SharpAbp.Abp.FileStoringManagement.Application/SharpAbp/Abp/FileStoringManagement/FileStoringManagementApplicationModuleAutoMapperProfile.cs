using AutoMapper;
using SharpAbp.Abp.FileStoring;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementApplicationModuleAutoMapperProfile : Profile
    {
        public FileStoringManagementApplicationModuleAutoMapperProfile()
        {
            CreateMap<FileProviderConfiguration, ProviderDto>();

            CreateMap<FileStoringContainer, ContainerDto>();
            CreateMap<FileStoringContainerItem, ContainerItemDto>();

            CreateMap<CreateContainerDto, FileStoringContainer>();
            CreateMap<UpdateContainerDto, FileStoringContainer>();
            
            CreateMap<ContainerItemDto, FileStoringContainerItem>();
        }
    }
}
