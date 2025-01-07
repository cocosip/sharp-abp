﻿using FASTER.core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Volo.Abp.Reflection;
using Volo.Abp.Serialization;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Faster
{
    public class FasterLogger<T> : IFasterLogger<T> where T : class
    {

        private long _completedUntilAddress = -1L;
        private bool _initialized = false;
        private readonly ConcurrentDictionary<long, LogEntryPosition> _pending;
        private readonly ConcurrentDictionary<long, LogEntryPosition> _committing;
        protected Channel<BufferedLogEntry> _pendingChannel;


        protected ILogger Logger { get; }
        protected AbpFasterOptions Options { get; }
        protected AbpFasterConfiguration Configuration { get; }
        protected IObjectSerializer Serializer { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }
        protected FasterLog Log { get; set; }
        protected FasterLogScanIterator Iter { get; set; }
        public bool Initialized { get { return _initialized; } }
        public string Name { get; }


        public FasterLogger(
            ILogger<FasterLogger<T>> logger,
            IOptions<AbpFasterOptions> options,
            string name,
            AbpFasterConfiguration configuration,
            IObjectSerializer serializer,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = logger;
            Options = options.Value;
            Name = name;
            Configuration = configuration;
            Serializer = serializer;
            CancellationTokenProvider = cancellationTokenProvider;

            _pendingChannel = Channel.CreateBounded<BufferedLogEntry>(Configuration.PreReadCapacity);
            _pending = new ConcurrentDictionary<long, LogEntryPosition>();
            _committing = new ConcurrentDictionary<long, LogEntryPosition>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            if (_initialized)
            {
                Logger.LogWarning("Faster log {Name} has initialized... ", TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                return;
            }

            var fileName = Path.Combine(Options.RootPath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), Configuration.FileName);

            var device = Devices.CreateLogDevice(
                fileName,
                Configuration.PreallocateFile,
                false,
                Configuration.Capacity,
                Configuration.RecoverDevice,
                Configuration.UseIoCompletionPort,
                Configuration.DisableFileBuffering);

            var settings = new FasterLogSettings()
            {
                LogDevice = device,
            };

            Configuration.Configure?.Invoke(settings);
            Log = new FasterLog(settings);

            Iter = Log.Scan(Log.BeginAddress, long.MaxValue, Configuration.IteratorName, true, ScanBufferingMode.DoublePageBuffering, Configuration.ScanUncommitted, Logger);

            _completedUntilAddress = Iter.CompletedUntilAddress;

            //commit
            StartScheduleCommit();

            //scan
            StartScheduleScan();

            //complete
            StartScheduleComplete();

            _initialized = true;

            Logger.LogDebug("Faster log {Name} is initialize. ", TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
        }

        /// <summary>
        /// 定时提交写入数据,避免因程序关闭等原因造成的数据丢失
        /// </summary>
        protected virtual void StartScheduleCommit()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    await Task.Delay(Configuration.CommitIntervalMillis, CancellationTokenProvider.Token);
                    try
                    {
                        await Log.CommitAsync(CancellationTokenProvider.Token);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Commit exception : {Message}", ex.Message);
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 扫描读取的文件迭代器中的数据
        /// </summary>
        protected virtual void StartScheduleScan()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    await Task.Delay(Configuration.ScanIntervalMillis, CancellationTokenProvider.Token);
                    try
                    {
                        byte[] buffer;
                        int entryLength;
                        long currentAddress;

                        while (!Iter.GetNext(out buffer, out entryLength, out currentAddress))
                        {
                            await Iter.WaitAsync(CancellationTokenProvider.Token);
                        }

                        if (buffer != null)
                        {
                            //写入到临时
                            var entry = new BufferedLogEntry
                            {
                                Data = buffer,
                                EntryLength = entryLength,
                                CurrentAddress = currentAddress,
                            };
                            await _pendingChannel.Writer.WriteAsync(entry, CancellationTokenProvider.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "ScheduleScan exception {Message}", ex.Message);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }


        protected virtual void StartScheduleComplete()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {

                    try
                    {
                        long commitAddress = _completedUntilAddress;
                        var commitList = _committing.Select(x => x.Value).OrderBy(x => x.Min).ToList();
                        foreach (var commit in commitList)
                        {
                            if (!commit.Min.IsNext(commitAddress))
                            {
                                break;
                            }
                            else
                            {
                                commitAddress = commit.Max.Address;
                            }
                        }

                        if (commitAddress > _completedUntilAddress)
                        {
                            Logger.LogDebug("CompleteUntilRecordAtAsync {commitAddress} .", commitAddress);
                            await Iter.CompleteUntilRecordAtAsync(commitAddress);
                            _completedUntilAddress = Iter.CompletedUntilAddress;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Commit exception : {Message}", ex.Message);
                    }

                    await Task.Delay(Configuration.CompleteIntervalMillis, CancellationTokenProvider.Token);
                }

            }, TaskCreationOptions.LongRunning);
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<long> WriteAsync(T entity, CancellationToken cancellationToken = default)
        {
            var buffer = Serializer.Serialize(entity);
            return await Log.EnqueueAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<LogEntryList<T>> ReadAsync(int count = 1, CancellationToken cancellationToken = default)
        {
            var entries = new LogEntryList<T>();

            if (count > 0)
            {
                while (entries.Count < count && !cancellationToken.IsCancellationRequested)
                {
                    BufferedLogEntry entry;
                    if (entries.Count == 0)
                    {
                        entry = await _pendingChannel.Reader.ReadAsync(cancellationToken);
                    }
                    else
                    {
                        if (!_pendingChannel.Reader.TryRead(out entry))
                        {
                            break;
                        }
                    }

                    entries.Add(new LogEntry<T>
                    {
                        Data = Serializer.Deserialize<T>(entry.Data),
                        CurrentAddress = entry.CurrentAddress,
                        EntryLength = entry.EntryLength,
                    });
                }
            }

            var position = entries.GetPosition();
            if (!_pending.TryAdd(position.Min.Address, position))
            {
                Logger.LogDebug("Add position to pending failed. {Address}", position.Min.Address);
            }
            return entries;
        }

        /// <summary>
        /// 提交进度
        /// </summary>
        /// <param name="entryPosition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task CommitAsync(LogEntryPosition entryPosition, CancellationToken cancellationToken = default)
        {
            if (_committing.TryAdd(entryPosition.Min.Address, entryPosition))
            {
                Logger.LogDebug("Add position to committing failed. {Address}", entryPosition.Min.Address);
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Log?.Dispose();
            Iter?.Dispose();
        }

    }
}
