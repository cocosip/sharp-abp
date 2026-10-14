using System;
using System.Collections.Generic;
using Volo.Abp.AuditLogging;
using Volo.Abp.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace SharpAbp.Abp.AuditLogging
{
    public class AuditLoggingMapperlyTest : AuditLoggingApplicationTestBase
    {
        private const string PublicProperty = "MapperlyPublicProperty";
        private const string InternalProperty = "MapperlyInternalProperty";
        private readonly IObjectMapper<AuditLoggingApplicationModule> _mapper;

        public AuditLoggingMapperlyTest()
        {
            _mapper = GetRequiredService<IObjectMapper<AuditLoggingApplicationModule>>();
            ObjectExtensionManager.Instance.AddOrUpdateProperty<EntityChange, string>(PublicProperty);
            ObjectExtensionManager.Instance.AddOrUpdateProperty<EntityChangeDto, string>(PublicProperty);
            ObjectExtensionManager.Instance.AddOrUpdateProperty<EntityChangeWithUsernameDto, string>(PublicProperty);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Map_EntityChangeWithUsername_Preserves_Details(bool existingDestination)
        {
            var change = CreateEntityChange();
            var source = new EntityChangeWithUsername { EntityChange = change, UserName = "review-user" };
            var result = existingDestination
                ? _mapper.Map(source, new EntityChangeWithUsernameDto())
                : _mapper.Map<EntityChangeWithUsername, EntityChangeWithUsernameDto>(source);

            Assert.Equal(change.Id, result.Id);
            Assert.Equal(change.AuditLogId, result.AuditLogId);
            Assert.Equal(change.TenantId, result.TenantId);
            Assert.Equal(change.ChangeTime, result.ChangeTime);
            Assert.Equal(change.ChangeType, result.ChangeType);
            Assert.Equal(change.EntityId, result.EntityId);
            Assert.Equal(change.EntityTypeFullName, result.EntityTypeFullName);
            Assert.Equal("review-user", result.UserName);
            Assert.Null(result.EntityName);
            var property = Assert.Single(result.PropertyChanges);
            Assert.Equal("Name", property.PropertyName);
            Assert.Equal("before", property.OriginalValue);
            Assert.Equal("after", property.NewValue);
            Assert.Equal("public-value", result.ExtraProperties[PublicProperty]);
            Assert.False(result.ExtraProperties.ContainsKey(InternalProperty));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Map_AuditLog_Preserves_Comments_And_Checks_Nested_Properties(bool existingDestination)
        {
            AuditLog source = new TestAuditLog(CreateEntityChange());
            var result = existingDestination
                ? _mapper.Map(source, new AuditLogDto())
                : _mapper.Map<AuditLog, AuditLogDto>(source);

            Assert.Equal("review-comment", result.Comments);
            var change = Assert.Single(result.EntityChanges);
            Assert.Equal("entity-42", change.EntityId);
            Assert.Equal("public-value", change.ExtraProperties[PublicProperty]);
            Assert.False(change.ExtraProperties.ContainsKey(InternalProperty));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Map_EntityChange_Checks_Extra_Property_Definitions(bool existingDestination)
        {
            var source = CreateEntityChange();
            var result = existingDestination
                ? _mapper.Map(source, new EntityChangeDto())
                : _mapper.Map<EntityChange, EntityChangeDto>(source);

            Assert.Equal("public-value", result.ExtraProperties[PublicProperty]);
            Assert.False(result.ExtraProperties.ContainsKey(InternalProperty));
            Assert.NotSame(source.ExtraProperties, result.ExtraProperties);
        }

        private static EntityChange CreateEntityChange()
        {
            var change = new EntityChange(SimpleGuidGenerator.Instance, Guid.NewGuid(), new EntityChangeInfo
            {
                ChangeTime = new DateTime(2026, 10, 14, 10, 0, 0),
                ChangeType = EntityChangeType.Updated,
                EntityId = "entity-42",
                EntityTypeFullName = "Review.Entity",
                PropertyChanges = new List<EntityPropertyChangeInfo>
                {
                    new EntityPropertyChangeInfo
                    {
                        PropertyName = "Name",
                        PropertyTypeFullName = "System.String",
                        OriginalValue = "before",
                        NewValue = "after"
                    }
                }
            }, Guid.NewGuid());
            change.ExtraProperties[PublicProperty] = "public-value";
            change.ExtraProperties[InternalProperty] = "internal-value";
            return change;
        }

        private sealed class TestAuditLog : AuditLog
        {
            public TestAuditLog(EntityChange change)
            {
                Comments = "review-comment";
                EntityChanges = new List<EntityChange> { change };
                Actions = new List<AuditLogAction>();
            }
        }
    }
}
