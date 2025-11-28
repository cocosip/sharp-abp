

using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.FileStoringManagement
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileStoringContainerToFileStoringContainerEtoMapper : MapperBase<FileStoringContainer, FileStoringContainerEto>
    {
        [MapperIgnoreTarget(nameof(FileStoringContainerEto.EntityType))]
        [MapperIgnoreTarget(nameof(FileStoringContainerEto.KeysAsString))]
        [MapperIgnoreTarget(nameof(FileStoringContainerEto.Properties))]
        public override partial FileStoringContainerEto Map(FileStoringContainer source);


        [MapperIgnoreTarget(nameof(FileStoringContainerEto.EntityType))]
        [MapperIgnoreTarget(nameof(FileStoringContainerEto.KeysAsString))]
        [MapperIgnoreTarget(nameof(FileStoringContainerEto.Properties))]
        public override partial void Map(FileStoringContainer source, FileStoringContainerEto destination);

    }

}
