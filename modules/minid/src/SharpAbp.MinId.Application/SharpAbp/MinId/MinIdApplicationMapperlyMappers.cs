using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.MinId
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MinIdInfoToMinIdInfoDtoMapper : MapperBase<MinIdInfo, MinIdInfoDto>
    {
        public override partial MinIdInfoDto Map(MinIdInfo source);
        public override partial void Map(MinIdInfo source, MinIdInfoDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MinIdInfoToMinIdInfoCacheItemMapper : MapperBase<MinIdInfo, MinIdInfoCacheItem>
    {
        public override partial MinIdInfoCacheItem Map(MinIdInfo source);
        public override partial void Map(MinIdInfo source, MinIdInfoCacheItem destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MinIdTokenToMinIdTokenDtoMapper : MapperBase<MinIdToken, MinIdTokenDto>
    {
        public override partial MinIdTokenDto Map(MinIdToken source);
        public override partial void Map(MinIdToken source, MinIdTokenDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MinIdTokenToMinIdTokenCacheItemMapper : MapperBase<MinIdToken, MinIdTokenCacheItem>
    {
        public override partial MinIdTokenCacheItem Map(MinIdToken source);
        public override partial void Map(MinIdToken source, MinIdTokenCacheItem destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SegmentIdToSegmentIdDtoMapper : MapperBase<SegmentId, SegmentIdDto>
    {
        public override partial SegmentIdDto Map(SegmentId source);
        public override partial void Map(SegmentId source, SegmentIdDto destination);
    }
}
