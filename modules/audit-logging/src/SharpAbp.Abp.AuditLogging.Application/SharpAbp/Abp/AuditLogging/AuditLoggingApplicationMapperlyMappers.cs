using Riok.Mapperly.Abstractions;
using Volo.Abp.AuditLogging;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.AuditLogging
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    [MapExtraProperties]
    public partial class AuditLogToAuditLogDtoMapper : MapperBase<AuditLog, AuditLogDto>
    {
        public override partial AuditLogDto Map(AuditLog source);
        public override partial void Map(AuditLog source, AuditLogDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
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
    [MapExtraProperties]
    public partial class EntityChangeWithUsernameToEntityChangeWithUsernameDtoMapper : MapperBase<EntityChangeWithUsername, EntityChangeWithUsernameDto>
    {
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.AuditLogId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.TenantId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ChangeTime))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ChangeType))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityTypeFullName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.PropertyChanges))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.Id))]
        public override partial EntityChangeWithUsernameDto Map(EntityChangeWithUsername source);

        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.AuditLogId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.TenantId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ChangeTime))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.ChangeType))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityId))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.EntityTypeFullName))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.PropertyChanges))]
        [MapperIgnoreTarget(nameof(EntityChangeWithUsernameDto.Id))]
        public override partial void Map(EntityChangeWithUsername source, EntityChangeWithUsernameDto destination);
    }

}
