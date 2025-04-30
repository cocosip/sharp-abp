using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public static class NameValueExtensions
    {
        public static NameValue FindOrDefault(this IList<NameValue> values, string name)
        {
            return values.FirstOrDefault(x => x.Name == name);
        }

        public static string FindValue(this IList<NameValue> values, string name)
        {
            return values.FirstOrDefault(x => x.Name == name)?.Value ?? "";
        }
    }
}
