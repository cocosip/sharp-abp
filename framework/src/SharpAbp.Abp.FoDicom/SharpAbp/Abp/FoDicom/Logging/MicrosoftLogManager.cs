using Microsoft.Extensions.Logging;

namespace SharpAbp.Abp.FoDicom.Log
{
    /// <summary>
    /// Microsoft logging adapter
    /// </summary>
    public class MicrosoftLogManager : FellowOakDicom.Log.LogManager
    {
        private readonly ILoggerFactory _loggerFactory;
        public MicrosoftLogManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override FellowOakDicom.Log.Logger GetLoggerImpl(string name)
        {
            return new MicrosoftLogger(_loggerFactory.CreateLogger(name));
        }
    }
}
