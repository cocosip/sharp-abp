using Microsoft.Extensions.Logging;
using System;

namespace SharpAbp.FoDicom.Logging
{
    /// <summary>微软日志记录器
    /// </summary>
    public class MicrosoftLogger : Dicom.Log.Logger
    {
        private readonly ILogger _microsoftLogger;

        public MicrosoftLogger(ILogger microsoftLogger)
        {
            _microsoftLogger = microsoftLogger;
        }

        public override void Log(Dicom.Log.LogLevel level, string msg, params object[] args)
        {
            var microsoftLogLevel = GetMicrosoftLogLevel(level);

            if (args.Length >= 1 && args[0] is Exception exception)
            {
                _microsoftLogger.Log(microsoftLogLevel, exception, msg, args);
            }
            else
            {
                _microsoftLogger.Log(microsoftLogLevel, msg, args);
            }
        }

        /// <summary>转换为微软日志记录级别
        /// </summary>
        public Microsoft.Extensions.Logging.LogLevel GetMicrosoftLogLevel(Dicom.Log.LogLevel level)
        {
            switch (level)
            {
                case Dicom.Log.LogLevel.Debug:
                    return LogLevel.Debug;
                case Dicom.Log.LogLevel.Error:
                    return LogLevel.Error;
                case Dicom.Log.LogLevel.Fatal:
                    return LogLevel.Critical;
                case Dicom.Log.LogLevel.Info:
                    return LogLevel.Information;
                case Dicom.Log.LogLevel.Warning:
                    return LogLevel.Warning;
                default:
                    //pathological case - shouldn't occur
                    return LogLevel.Trace;
            }
        }
    }
}
