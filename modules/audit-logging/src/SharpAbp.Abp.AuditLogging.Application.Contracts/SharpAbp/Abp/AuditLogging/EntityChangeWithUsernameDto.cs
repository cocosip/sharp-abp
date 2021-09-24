namespace SharpAbp.Abp.AuditLogging
{
    public class EntityChangeWithUsernameDto
    {
        public EntityChangeDto EntityChange { get; set; }

        public string UserName { get; set; }
    }
}
