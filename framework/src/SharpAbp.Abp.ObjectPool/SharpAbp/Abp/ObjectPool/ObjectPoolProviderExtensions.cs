using System;
using Microsoft.Extensions.ObjectPool;

namespace SharpAbp.Abp.ObjectPool
{
    public static class ObjectPoolProviderExtensions
    {
        public static int GetDefaultSize(this ObjectPoolProvider provider)
        {
            return Environment.ProcessorCount * 2;
        }
    }
}
