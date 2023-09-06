using System.Collections.Generic;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class SharpAbpEfCoreOptions
    {
        public EfCoreDatabaseProvider DatabaseProvider { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public SharpAbpEfCoreOptions()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
