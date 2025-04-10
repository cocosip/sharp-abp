﻿using FASTER.core;
using System;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterConfiguration
    {
        /// <summary>
        /// 记录数据的文件名
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Whether we try to preallocate the file on creation
        /// </summary>
        public bool PreallocateFile { get; set; }

        /// <summary>
        /// 存储空间 (4GB)
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
        /// 扫描未写入完成到页面的
        /// </summary>
        public bool ScanUncommitted { get; set; } = false;

        /// <summary>
        /// AutoRefreshSafeTailAddress
        /// </summary>
        public bool AutoRefreshSafeTailAddress { get; set; } = false;

        /// <summary>
        /// Page Size, 20(1MB),原生默认 22(4MB)
        /// </summary>
        public int PageSizeBits { get; set; } = 22;

        /// <summary>
        /// Memory Size, 21(2MB), 原生默认 23(8MB)
        /// </summary>
        public int MemorySizeBits { get; set; } = 23;

        /// <summary>
        /// SegmentSize, 28(256MB), 原生默认 30(1GB)
        /// </summary>
        public int SegmentSizeBits { get; set; } = 30;

        /// <summary>
        /// 提交数据的时间间隔(ms)
        /// </summary>
        public int CommitIntervalMillis { get; set; } = 2000;

        /// <summary>
        /// 完成数据的时间间隔(ms)
        /// </summary>
        public int CompleteIntervalMillis { get; set; } = 3000;

        /// <summary>
        /// 日志截断的时间间隔(s)
        /// </summary>
        public int TruncateIntervalMillis { get; set; } = 60 * 5 * 1000;

        /// <summary>
        /// 预先从Faster读取的数据的容量(ms)
        /// </summary>
        public int PreReadCapacity { get; set; } = 5000;

        /// <summary>
        /// 最大提交跳过
        /// </summary>
        public int MaxCommitSkip { get; set; } = 50;

        /// <summary>
        /// 迭代器的名称
        /// </summary>
        public string IteratorName { get; set; } = "default";

        /// <summary>
        /// 配置
        /// </summary>
        public Action<FasterLogSettings>? Configure { get; set; }
    }
}
