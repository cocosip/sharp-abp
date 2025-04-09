using DotCommon.Scheduling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultPerformanceService : IPerformanceService
    {
        public string Name { get; }
        protected string TaskName { get; }

        public PerformanceConfiguration Configuration { get; }
        protected ILogger Logger { get; }
        protected ICountInfoFactory CountInfoFactory { get; }
        protected IScheduleService ScheduleService { get; }

        private readonly ConcurrentDictionary<string, CountInfo> _countInfoDict;
        public DefaultPerformanceService(
            ILogger<DefaultPerformanceService> logger,
            ICountInfoFactory countInfoFactory,
            IScheduleService scheduleService,
            string name,
            PerformanceConfiguration configuration)
        {
            Logger = logger;
            CountInfoFactory = countInfoFactory;
            ScheduleService = scheduleService;
            Configuration = configuration;
            Name = name;
            TaskName = name + ".Task";

            _countInfoDict = new ConcurrentDictionary<string, CountInfo>();
        }

        /// <summary>
        /// Start
        /// </summary>
        public virtual void Start()
        {
            if (TaskName.IsNullOrWhiteSpace())
            {
                throw new Exception(string.Format("Please initialize the {0} before start it.", GetType().FullName));
            }

            ScheduleService.StartTask(TaskName, () =>
            {
                foreach (var entry in _countInfoDict)
                {
                    entry.Value.Calculate();
                }
            }, Configuration.StatIntervalSeconds * 1000, Configuration.StatIntervalSeconds * 1000);
        }

        /// <summary>
        /// Stop
        /// </summary>
        public virtual void Stop()
        {
            if (TaskName.IsNullOrWhiteSpace())
            {
                return;
            }
            ScheduleService.StopTask(TaskName);
        }

        /// <summary>
        /// Increament key count
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rtMilliseconds"></param>
        public virtual void IncrementKeyCount(string key, double rtMilliseconds)
        {
            _countInfoDict.AddOrUpdate(key,
            x =>
            {
                return CountInfoFactory.Create(Name, key, Configuration, 1, rtMilliseconds);
            },
            (x, y) =>
            {
                y.IncrementTotalCount(rtMilliseconds);
                return y;
            });
        }

        /// <summary>
        /// Update key count
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <param name="rtMilliseconds"></param>
        public virtual void UpdateKeyCount(string key, long count, double rtMilliseconds)
        {
            _countInfoDict.AddOrUpdate(key,
            x =>
            {
                return CountInfoFactory.Create(Name, key, Configuration, count, rtMilliseconds);
            },
            (x, y) =>
            {
                y.UpdateTotalCount(count, rtMilliseconds);
                return y;
            });
        }

        /// <summary>
        /// Get key performanceInfo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual PerformanceInfo? GetKeyPerformanceInfo(string key)
        {
            CountInfo countInfo;
            if (_countInfoDict.TryGetValue(key, out countInfo))
            {
                return countInfo.GetCurrentPerformanceInfo();
            }
            return null;
        }

    }
}
