using AutoMapper;
using SharpAbp.Abp.FileStoring;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementApplicationModuleAutoMapperProfile : Profile
    {
        public FileStoringManagementApplicationModuleAutoMapperProfile()
        {
            CreateMap<FileProviderConfiguration, ProviderDto>();

            CreateMap<FileStoringContainer, FileStoringContainerDto>();
            CreateMap<FileStoringContainerItem, FileStoringContainerItemDto>();

            CreateMap<CreateContainerInput, FileStoringContainer>();
            CreateMap<CreateContainerItemInput, FileStoringContainerItem>();

            CreateMap<UpdateContainerInput, FileStoringContainer>();
            CreateMap<UpdateContainerItemInput, FileStoringContainerItem>();
        }
    }
}
