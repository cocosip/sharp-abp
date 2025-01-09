using Microsoft.Extensions.Logging;
using SharpAbp.Abp.Faster;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace FasterLogSample;

public class FasterLogService : ISingletonDependency
{
    protected ILogger Logger { get; set; }
    protected IClock Clock { get; set; }
    protected IFasterLoggerFactory LoggerFactory { get; set; }
    protected IFasterLogger<TenantData> FasterLogger { get; set; }
    protected ICancellationTokenProvider CancellationTokenProvider { get; set; }

    public FasterLogService(
        ILogger<FasterLogService> logger,
        IClock clock,
        IFasterLoggerFactory loggerFactory,
        ICancellationTokenProvider cancellationTokenProvider)
    {
        Logger = logger;
        Clock = clock;
        LoggerFactory = loggerFactory;
        FasterLogger = LoggerFactory.GetOrCreate<TenantData>("default");
        CancellationTokenProvider = cancellationTokenProvider;
    }


    public void Start()
    {
        Write();
        Read();
    }


    public void Write()
    {
        Task.Factory.StartNew(async () =>
        {
            var data = new TenantData()
            {
                TenantId = "3a0f7d34-2a3d-90d3-6f92-07ebc4f1230d",
                StudyInstanceUID = "1.3.1890.20241101.1104754694.140.86578",
                SeriesInstanceUID = "1.2.156.112605.66988328761091.241102005416.3.8720.104750",
                SOPInstanceUIDP = "1.2.156.112605.66988328761091.241102005607.4.1836.117740",
                FileSize = 526936,
                FileId = "Hidos/Default/1.3.1890.20241101.1104754694.140.86578/1.2.156.112605.66988328761091.241102005416.3.8720.104750-CT/1.2.156.112605.66988328761091.241102005607.4.1836.117740-301-12.dcm",
            };

            while (!CancellationTokenProvider.Token.IsCancellationRequested)
            {
                try
                {
                    data.CreationTime = Clock.Now;
                    await FasterLogger.WriteAsync(data, CancellationTokenProvider.Token);

                    var d = data.CreationTime.ToString("yyyy-MM-dd HH:mm:ss fff");

                    Logger.LogInformation("Write data: {d}", d);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Write data error -> {Message}", ex.Message);
                }

                await Task.Delay(1);
            }

        }, TaskCreationOptions.LongRunning);
    }

    public void Read()
    {
        for (var i = 0; i < 3; i++)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3000);

                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    try
                    {
                        var list = await FasterLogger.ReadAsync(5, CancellationTokenProvider.Token);

                        var min = list.Min(x => x.Data.CreationTime);
                        var max = list.Max(x => x.Data.CreationTime);
                        Logger.LogDebug("Read data min: {min}, max:{max}", min.ToString("yyyy-MM-dd HH:mm:ss fff"), max.ToString("yyyy-MM-dd HH:mm:ss fff"));

                        var p = list.GetPosition();
                        //提交
                        await FasterLogger.CommitAsync(p, CancellationTokenProvider.Token);

                        await Task.Delay(5000);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Read data error -> {Message}", ex.Message);
                    }

                    await Task.Delay(20);
                }



            }, TaskCreationOptions.LongRunning);
        }
    }





    public class TenantData
    {
        public string TenantId { get; set; }

        public string StudyInstanceUID { get; set; }

        public string SeriesInstanceUID { get; set; }
        public string SOPInstanceUIDP { get; set; }
        public long FileSize { get; set; }

        public string FileId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
