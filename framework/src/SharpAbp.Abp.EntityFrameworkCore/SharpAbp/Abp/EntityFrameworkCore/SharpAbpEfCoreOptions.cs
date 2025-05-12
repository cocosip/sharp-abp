using System.Collections.Generic;
using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    public class SharpAbpEfCoreOptions
    {
        public DatabaseProvider DatabaseProvider { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public SharpAbpEfCoreOptions()
        {
            Properties = [];
        }
    }
}
