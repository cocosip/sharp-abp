using Riok.Mapperly.Abstractions;
using Volo.Abp.AuditLogging;
using Volo.Abp.Mapperly;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.AuditLogging
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target,
        IncludedMembers = MemberVisibility.Public | MemberVisibility.Protected)]
    [MapExtraProperties]
    public partial class AuditLogToAuditLogDtoMapper : MapperBase<AuditLog, AuditLogDto>
    {
        private readonly IObjectMapper<AuditLoggingApplicationModule> _objectMapper;

        public AuditLogToAuditLogDtoMapper(IObjectMapper<AuditLoggingApplicationModule> objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public override partial AuditLogDto Map(AuditLog source);
        public override partial void Map(AuditLog source, AuditLogDto destination);

        // Nested extensible objects must pass through ABP's property definition checks.
        private EntityChangeDto MapEntityChange(EntityChange source)
            => _objectMapper.Map<EntityChange, EntityChangeDto>(source);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    [MapExtraProperties]
    public partial class EntityChangeToEntityChangeDtoMapper : MapperBase<EntityChange, EntityChangeDto>
    {
        public override partial EntityChangeDto Map(EntityChange source);
        public override partial void Map(EntityChange source, EntityChangeDto destination);
    }


    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class EntityPropertyChangeToEntityPropertyChangeDtoMapper : MapperBase<EntityPropertyChange, EntityPropertyChangeDto>
    {
        public override partial EntityPropertyChangeDto Map(EntityPropertyChange source);
        public override partial void Map(EntityPropertyChange source, EntityPropertyChangeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class AuditLogActionToAuditLogActionDtoMapper : MapperBase<AuditLogAction, AuditLogActionDto>
    {
        public override partial AuditLogActionDto Map(AuditLogAction source);
        public override partial void Map(AuditLogAction source, AuditLogActionDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class EntityChangeWithUsernameToEntityChangeWithUsernameDtoMapper : MapperBase<EntityChangeWithUsername, EntityChangeWithUsernameDto>
    {
        [MapNestedProperties(nameof(EntityChangeWithUsername.EntityChange))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ExtraProperties))]
        public override partial EntityChangeWithUsernameDto Map(EntityChangeWithUsername source);

        [MapNestedProperties(nameof(EntityChangeWithUsername.EntityChange))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ExtraProperties))]
        public override partial void Map(EntityChangeWithUsername source, EntityChangeWithUsernameDto destination);

        public override void AfterMap(EntityChangeWithUsername source, EntityChangeWithUsernameDto destination)
        {
            source.EntityChange?.MapExtraPropertiesTo(destination);
        }
    }

}
