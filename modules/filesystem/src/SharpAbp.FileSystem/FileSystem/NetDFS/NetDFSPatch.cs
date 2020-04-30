using System;

namespace SharpAbp.FileSystem.NetDFS
{
    /// <summary>NetDFS网络存储Patch
    /// </summary>
    [Serializable]
    public class NetDFSPatch : Patch
    {
        public NetDFSPatch()
        {
            StoreType = (int) SharpAbp.FileSystem.StoreType.NetDFS;
        }

    }
}
