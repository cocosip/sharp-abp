using System;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Serializable]
    public class DatabaseConnectionNameChangedEto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OldName { get; set; }
    }
}
