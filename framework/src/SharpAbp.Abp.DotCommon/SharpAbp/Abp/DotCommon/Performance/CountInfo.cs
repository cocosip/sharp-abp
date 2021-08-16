using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class CountInfo
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _name;
        private readonly string _key;

        private readonly PerformanceConfiguration _configuration;

        private long _totalCount;
        private long _previousCount;
        private long _throughput;
        private long _averageThroughput;
        private long _throughputCalculateCount;

        private long _rtCount;
        private long _totalRTTime;
        private long _rtTime;
        private double _rt;
        private double _averateRT;
        private long _rtCalculateCount;

        public long TotalCount
        {
            get { return _totalCount; }
        }
        public long Throughput
        {
            get { return _throughput; }
        }
        public long AverageThroughput
        {
            get { return _averageThroughput; }
        }
        public double RT
        {
            get { return _rt; }
        }
        public double AverageRT
        {
            get { return _averateRT; }
        }

        public CountInfo(
            ILogger<CountInfo> logger,
            IServiceProvider serviceProvider,
            string name,
            string key,
            PerformanceConfiguration configuration,
            long initialCount,
            double rtMilliseconds)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _name = name;
            _key = key;
            _configuration = configuration;

            _totalCount = initialCount;
            _rtCount = initialCount;
            Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
            Interlocked.Add(ref _totalRTTime, (long)(rtMilliseconds * 1000));
        }

        public void IncrementTotalCount(double rtMilliseconds)
        {
            Interlocked.Increment(ref _totalCount);
            Interlocked.Increment(ref _rtCount);
            Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
            Interlocked.Add(ref _totalRTTime, (long)(rtMilliseconds * 1000));
        }
        public void UpdateTotalCount(long count, double rtMilliseconds)
        {
            _totalCount = count;
            _rtCount = count;
            Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
            Interlocked.Add(ref _totalRTTime, (long)(rtMilliseconds * 1000));
        }
        public void Calculate()
        {
            CalculateThroughput();
            CalculateRT();

            if (_configuration.AutoLogging)
            {
                var contextText = GetLogContextText();
                if (!contextText.IsNullOrWhiteSpace())
                {
                    contextText += ", ";
                }
                _logger.LogInformation("{0}-{1}, {2}totalCount: {3}, throughput: {4}, averageThrughput: {5}, rt: {6:F3}ms, averageRT: {7:F3}ms", _name, _key, contextText, _totalCount, _throughput, _averageThroughput, _rt, _averateRT);
            }

            if (_configuration.PerformanceInfoHandlers.Any())
            {
                try
                {
                    PerformanceInfoHandler(GetCurrentPerformanceInfo());
                }
                catch (Exception ex)
                {
                    _logger.LogError("PerformanceInfo handler execution has exception.", ex);
                }
            }
        }
        public PerformanceInfo GetCurrentPerformanceInfo()
        {
            return new PerformanceInfo(TotalCount, Throughput, AverageThroughput, RT, AverageRT);
        }

        private string GetLogContextText()
        {
            if (_configuration.LogContextTexts.Any())
            {
                using var scope = _serviceProvider.CreateScope();
                foreach (var logContextTextType in _configuration.LogContextTexts)
                {
                    var logContextTextService = scope.ServiceProvider
                        .GetRequiredService(logContextTextType)
                        .As<ILogContextTextService>();

                    return logContextTextService.GetLogContextText(_name, _key);
                }
            }
            return "";
        }

        private void PerformanceInfoHandler(PerformanceInfo performanceInfo)
        {
            if (_configuration.PerformanceInfoHandlers.Any())
            {
                using var scope = _serviceProvider.CreateScope();
                foreach (var performanceInfoHandlerType in _configuration.PerformanceInfoHandlers)
                {
                    var performanceInfoHandlerService = scope.ServiceProvider
                        .GetRequiredService(performanceInfoHandlerType)
                        .As<IPerformanceInfoHandlerService>();

                    performanceInfoHandlerService.Handle(_name, _key, performanceInfo);
                }
            }
        }

        private void CalculateThroughput()
        {
            var totalCount = _totalCount;
            _throughput = totalCount - _previousCount;
            _previousCount = totalCount;

            if (_throughput > 0)
            {
                _throughputCalculateCount++;
                _averageThroughput = totalCount / _throughputCalculateCount;
            }
        }
        private void CalculateRT()
        {
            var rtCount = _rtCount;
            var rtTime = _rtTime;
            var totalRTTime = _totalRTTime;

            if (rtCount > 0)
            {
                _rt = ((double)rtTime / 1000) / rtCount;
                _rtCalculateCount += rtCount;
                _averateRT = ((double)totalRTTime / 1000) / _rtCalculateCount;
            }

            _rtCount = 0L;
            _rtTime = 0L;
        }
    }
}
