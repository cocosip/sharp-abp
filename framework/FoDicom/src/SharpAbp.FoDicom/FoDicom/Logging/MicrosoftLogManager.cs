using Dicom.Log;
using Microsoft.Extensions.Logging;

namespace SharpAbp.FoDicom.Logging
{
    /// <summary>微软日志适配
    /// </summary>
    public class MicrosoftLogManager : Dicom.Log.LogManager
    {
        private readonly ILoggerFactory _loggerFactory;
        public MicrosoftLogManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override Logger GetLoggerImpl(string name)
        {
            return new MicrosoftLogger(_loggerFactory.CreateLogger(name));
        }

    }
}
