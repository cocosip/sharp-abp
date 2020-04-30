using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem.Impl
{
    /// <summary>默认的查询
    /// </summary>
    public class EmptyFileSystemQuery : IFileSystemQuery
    {
         public Task<Patch> FindPatchByPatchNameAsync(string code, StoreType storeType, string patchName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Patch>> FindPatchsAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<List<StoreType>> FindStoreTypesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
