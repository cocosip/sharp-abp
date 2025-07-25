﻿using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Serializable]
    public class DatabaseConnectionUpdatedEto : EtoBase
    {
        public Guid Id { get; set; }
        public string OldName { get; set; }
        public string Name { get; set; }
        public string DatabaseProvider { get; set; }
        public string ConnectionString { get; set; }
    }
}
