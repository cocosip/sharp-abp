using System;

namespace SharpAbp.Abp.ObjectPool
{
    public static class ObjectPoolDefaults
    {
        /// <summary>
        /// Gets the default maximum size for object pools.
        /// </summary>
        /// <returns>The default size based on processor count.</returns>
        public static int GetDefaultMaximumRetained()
        {
            return Environment.ProcessorCount * 2;
        }
    }
}
