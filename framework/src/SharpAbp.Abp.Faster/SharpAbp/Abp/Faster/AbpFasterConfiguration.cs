using FASTER.core;
using System;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterConfiguration
    {
        /// <summary>
        /// 记录数据的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Whether we try to preallocate the file on creation
        /// </summary>
        public bool PreallocateFile { get; set; }

        /// <summary>
        /// 存储空间
        /// </summary>
        public long Capacity { get; set; } = 1L << 32;

        /// <summary>
        /// Whether to recover device metadata from existing files
        /// </summary>
        public bool RecoverDevice { get; set; } = true;

        /// <summary>
        /// Whether we use IO completion port with polling
        /// </summary>
        public bool UseIoCompletionPort { get; set; } = false;

        /// <summary>
        /// Whether file buffering (during write) is disabled (default of true requires aligned writes)
        /// </summary>
        public bool DisableFileBuffering { get; set; } = true;

        /// <summary>
        /// 扫描写入的
        /// </summary>
        public bool ScanUncommitted { get; set; } = false;

        /// <summary>
        /// 提交数据的时间间隔(ms)
        /// </summary>
        public int CommitIntervalMillis { get; set; } = 1000;

        /// <summary>
        /// 扫描的时间间隔(ms)
        /// </summary>
        public int ScanIntervalMillis { get; set; } = 1000;

        /// <summary>
        /// 扫描不到数据等待时间
        /// </summary>
        public int ScanWaitMillis { get; set; } = 1000;

        /// <summary>
        /// 完成数据的时间间隔(ms)
        /// </summary>
        public int CompleteIntervalMillis { get; set; } = 5000;

        /// <summary>
        /// 日志截断的时间间隔(s)
        /// </summary>
        public int TruncateIntervalSecond { get; set; } = 60;

        /// <summary>
        /// 预先从Faster读取的数据的容量(ms)
        /// </summary>
        public int PreReadCapacity { get; set; } = 5000;

        /// <summary>
        /// 自动过期数据的时间(ms)
        /// </summary>
        public int AutoRetiredMillis { get; set; } = 10000;

        /// <summary>
        /// 迭代器的名称
        /// </summary>
        public string IteratorName { get; set; } = "default";

        /// <summary>
        /// 配置
        /// </summary>
        public Action<FasterLogSettings> Configure { get; set; }
    }
}
