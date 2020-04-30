using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.FileSystem
{
    /// <summary>相关数据查询
    /// </summary>
    public interface IFileSystemQuery
    {

        /// <summary>获取全部的存储类型
        /// </summary>
        Task<List<StoreType>> FindStoreTypesAsync();

        /// <summary>查询Patchs
        /// </summary>
        Task<List<Patch>> FindPatchsAsync(string code);

        /// <summary>根据编码,存储类型,存储Patch获取当前Patch
        /// </summary>
        Task<Patch> FindPatchByPatchNameAsync(string code, StoreType storeType, string patchName);

    }
}
