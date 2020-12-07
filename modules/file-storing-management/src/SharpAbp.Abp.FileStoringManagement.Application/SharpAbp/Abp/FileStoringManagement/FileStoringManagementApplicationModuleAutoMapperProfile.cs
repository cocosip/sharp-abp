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

            CreateMap<CreateContainerInput, FileStoringContainer>();
            CreateMap<UpdateContainerInput, FileStoringContainer>();
            
            CreateMap<ContainerItemInput, FileStoringContainerItem>();
        }
    }
}
