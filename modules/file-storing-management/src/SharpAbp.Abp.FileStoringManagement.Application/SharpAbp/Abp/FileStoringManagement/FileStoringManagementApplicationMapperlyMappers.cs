using Riok.Mapperly.Abstractions;
using SharpAbp.Abp.FileStoring;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.FileStoringManagement
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileProviderConfigurationToProviderDtoMapper : MapperBase<FileProviderConfiguration, ProviderDto>
    {
        public override partial ProviderDto Map(FileProviderConfiguration source);
        public override partial void Map(FileProviderConfiguration source, ProviderDto destination);
    }


    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileStoringContainerToContainerDtoMapper : MapperBase<FileStoringContainer, ContainerDto>
    {
        public override partial ContainerDto Map(FileStoringContainer source);
        public override partial void Map(FileStoringContainer source, ContainerDto destination);
    }


    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Both)]
    public partial class FileStoringContainerItemToContainerItemDtoMapper : TwoWayMapperBase<FileStoringContainerItem, ContainerItemDto>
    {

        public override partial ContainerItemDto Map(FileStoringContainerItem source);

        public override partial void Map(FileStoringContainerItem source, ContainerItemDto destination);

        public override partial FileStoringContainerItem ReverseMap(ContainerItemDto destination);

        public override partial void ReverseMap(ContainerItemDto destination, FileStoringContainerItem source);
    }


    // public class FileStoringManagementApplicationModuleAutoMapperProfile : Profile
    // {
    //     public FileStoringManagementApplicationModuleAutoMapperProfile()
    //     {
    //         CreateMap<FileProviderConfiguration, ProviderDto>();

    //         CreateMap<FileStoringContainer, ContainerDto>();


    //         CreateMap<CreateContainerDto, FileStoringContainer>();
    //         CreateMap<UpdateContainerDto, FileStoringContainer>();

    //         CreateMap<FileStoringContainerItem, ContainerItemDto>();
    //         CreateMap<ContainerItemDto, FileStoringContainerItem>();
    //     }
    // }
}
