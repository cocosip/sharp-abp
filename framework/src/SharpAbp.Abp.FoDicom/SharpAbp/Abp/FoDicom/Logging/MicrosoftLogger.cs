using Microsoft.Extensions.Logging;
using System;

namespace SharpAbp.Abp.FoDicom.Log
{
    /// <summary>微软日志记录器
    /// </summary>
    public class MicrosoftLogger : FellowOakDicom.Log.Logger
    {

        private readonly ILogger _microsoftLogger;

        public MicrosoftLogger(ILogger microsoftLogger)
        {
            _microsoftLogger = microsoftLogger;
        }

        public override void Log(FellowOakDicom.Log.LogLevel level, string msg, params object[] args)
        {
            var microsoftLogLevel = GetMicrosoftLogLevel(level);

            if (args.Length >= 1 && args[0] is Exception)
            {
                _microsoftLogger.Log(microsoftLogLevel, (Exception)args[0], msg, args);
            }
            else
            {
                _microsoftLogger.Log(microsoftLogLevel, msg, args);
            }
        }


        /// <summary>转换为微软日志记录级别
        /// </summary>
        public LogLevel GetMicrosoftLogLevel(FellowOakDicom.Log.LogLevel level)
        {
            switch (level)
            {
                case FellowOakDicom.Log.LogLevel.Debug:
                    return LogLevel.Debug;
                case FellowOakDicom.Log.LogLevel.Error:
                    return LogLevel.Error;
                case FellowOakDicom.Log.LogLevel.Fatal:
                    return LogLevel.Critical;
                case FellowOakDicom.Log.LogLevel.Info:
                    return LogLevel.Information;
                case FellowOakDicom.Log.LogLevel.Warning:
                    return LogLevel.Warning;
                default:
                    //pathological case - shouldn't occur
                    return LogLevel.Trace;
            }
        }
    }
}
