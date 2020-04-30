using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.FileSystem.Impl
{
    /// <summary>根据编码选择具体的Patch
    /// </summary>
    public class DefaultPatchSelector : IPatchSelector
    {

        /// <summary>选择对应的Bucket或者Group
        /// </summary>
        public Patch SelectPatch(string code, List<Patch> patchs)
        {
            if (!patchs.Any())
            {
                throw new ArgumentException("无法选择对应的Patch");
            }

            return patchs.FirstOrDefault();
        }

    }
}
