using System;
using System.Collections.Generic;

namespace SharpAbp.FileSystem.FastDFS
{
    /// <summary>FastDFS文件存储Patch
    /// </summary>
    [Serializable]
    public class FastDFSPatch : Patch
    {
        /// <summary>对应的Trackers信息
        /// </summary>
        public List<Tracker> Trackers { get; set; } = new List<Tracker>();

        /// <summary>Ctor
        /// </summary>
        public FastDFSPatch()
        {
            StoreType = (int) SharpAbp.FileSystem.StoreType.FastDFS;

        }


    }


}
