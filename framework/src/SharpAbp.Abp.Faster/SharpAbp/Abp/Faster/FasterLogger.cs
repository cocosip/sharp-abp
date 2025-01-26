using FASTER.core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private long _truncateUntilAddress = -1L;
        private bool _initialized = false;
        private readonly ConcurrentDictionary<long, Position> _committing;
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
            _committing = new ConcurrentDictionary<long, Position>();
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
                AutoRefreshSafeTailAddress = Configuration.AutoRefreshSafeTailAddress,
            };

            Configuration.Configure?.Invoke(settings);
            Log = new FasterLog(settings);

            Iter = Log.Scan(Log.BeginAddress, long.MaxValue, Configuration.IteratorName, true, ScanBufferingMode.DoublePageBuffering, Configuration.ScanUncommitted, Logger);

            _completedUntilAddress = Math.Max(Iter.CompletedUntilAddress, Iter.BeginAddress);

            //commit
            StartScheduleCommit();

            //scan
            StartScheduleScan();

            //complete
            StartScheduleComplete();

            //truncate
            StartScheduleTruncate();

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
                    try
                    {
                        byte[] buffer;
                        int entryLength;
                        long currentAddress;
                        long nextAddress;
                        while (!Iter.GetNext(out buffer, out entryLength, out currentAddress, out nextAddress))
                        {
                            await Iter.WaitAsync(CancellationTokenProvider.Token);
                        }


                        if (buffer != null && entryLength > 0)
                        {
                            //写入到临时
                            var entry = new BufferedLogEntry
                            {
                                Data = buffer,
                                EntryLength = entryLength,
                                CurrentAddress = currentAddress,
                                NextAddress = nextAddress
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

        /// <summary>
        /// 扫描自定完成的,并且进行数据的提交
        /// </summary>
        protected virtual void StartScheduleComplete()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {

                    try
                    {
                        long commitAddress = _completedUntilAddress;
                        var commitList = _committing.Select(x => x.Value).OrderBy(x => x.Address).ToList();
                        var removeIds = new List<long>();


                        foreach (var commit in commitList)
                        {
                            if (!commit.IsMatch(commitAddress))
                            {
                                break;
                            }
                            else
                            {
                                if (commitAddress <= Iter.EndAddress)
                                {
                                    commitAddress = commit.NextAddress;
                                    //添加到删除列表
                                    removeIds.Add(commit.Address);
                                }
                            }
                        }

                        if (commitAddress > _completedUntilAddress && commitAddress <= Iter.EndAddress)
                        {
                            Logger.LogDebug("CompleteUntilRecordAtAsync {commitAddress} .", commitAddress);
                            await Log.CommitAsync(CancellationTokenProvider.Token);
                            await Iter.CompleteUntilRecordAtAsync(commitAddress, CancellationTokenProvider.Token);
                         

                            //不能直接使用 Iter.CompletedUntilAddress,因为这时候还没刷新
                            //_completedUntilAddress = Iter.CompletedUntilAddress;
                            _completedUntilAddress = commitAddress;
                            //移除这个之前的数据
                            if (removeIds.Count > 0)
                            {
                                foreach (var id in removeIds)
                                {
                                    if (!_committing.TryRemove(id, out _))
                                    {
                                        Logger.LogWarning("Remove commit {id} failed.", id);
                                    }
                                }
                            }
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
        /// 定时截断已经完成的
        /// </summary>
        protected virtual void StartScheduleTruncate()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {

                    try
                    {
                        if (_truncateUntilAddress < _completedUntilAddress)
                        {
                            Log.TruncateUntilPageStart(_completedUntilAddress);
                            Logger.LogDebug("TruncateUntilPageStart {_completedUntilAddress}", _completedUntilAddress);
                            _truncateUntilAddress = _completedUntilAddress;
                        }
                        else
                        {
                            Logger.LogDebug("TruncateUntilPageStart  truncateUntilAddress:({_truncateUntilAddress}) >completedUntilAddress:({_completedUntilAddress})", _completedUntilAddress);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Truncate exception : {Message}", ex.Message);
                    }

                    await Task.Delay(Configuration.TruncateIntervalMillis, CancellationTokenProvider.Token);
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
        /// 批量写入数据
        /// </summary>
        /// <param name="values"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken cancellationToken = default)
        {
            var positions = new List<long>();
            foreach (var entity in values)
            {
                var buffer = Serializer.Serialize(entity);
                var p = await Log.EnqueueAsync(buffer, cancellationToken);
                positions.Add(p);
            }

            return positions;
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

            for (var i = 0; i < count; i++)
            {
                BufferedLogEntry entry;
                if (i == 0)
                {
                    entry = await _pendingChannel.Reader.ReadAsync(cancellationToken);
                    //entries.Add(new LogEntry<T>
                    //{
                    //    Data = Serializer.Deserialize<T>(entry.Data),
                    //    CurrentAddress = entry.CurrentAddress,
                    //    EntryLength = entry.EntryLength,
                    //    NextAddress = entry.NextAddress,
                    //});
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
                    NextAddress = entry.NextAddress,
                });
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
            foreach (var position in entryPosition)
            {
                if (!_committing.TryAdd(position.Address, position))
                {
                    Logger.LogDebug("Add position to committing failed. {Address}", position.Address);
                }
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
