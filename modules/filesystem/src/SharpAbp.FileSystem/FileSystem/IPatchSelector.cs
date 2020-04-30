using System.Collections.Generic;

namespace SharpAbp.FileSystem
{
    /// <summary>根据编码选择具体的Patch
    /// </summary>
    public interface IPatchSelector
    {
        /// <summary>选择对应的Bucket或者Group
        /// </summary>
        Patch SelectPatch(string code, List<Patch> patchs);
    }
}
