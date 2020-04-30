using DotCommon.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace SharpAbp.FileSystem.Impl
{
    /// <summary>默认文件路径存储生成器
    /// </summary>
    public class DefaultFileIdGenerator : IFileIdGenerator
    {
        private readonly ILogger _logger;
        private readonly FileSystemOption _option;
        public DefaultFileIdGenerator(ILogger<DefaultFileIdGenerator> logger, IOptions<FileSystemOption> option)
        {
            _logger = logger;
            _option = option.Value;
        }


        /// <summary>默认的存储路径生成器
        /// </summary>
        public string GenerateFileId(StoreType storeType, Patch patch, UploadFileInfo info)
        {
            var uploadPath = "";
            //如果是FastDFS则返回空值
            if (storeType != StoreType.FastDFS)
            {
                uploadPath = $"{DateTimeUtil.GetPadDay(DateTime.Now)}/{ObjectId.GenerateNewStringId()}/{info.FileExt}";
            }
            return uploadPath;
        }

    }
}
