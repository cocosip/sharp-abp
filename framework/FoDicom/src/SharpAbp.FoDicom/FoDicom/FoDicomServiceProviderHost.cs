using FellowOakDicom;
using System;

namespace SharpAbp.FoDicom
{
    public class FoDicomServiceProviderHost : IServiceProviderHost
    {
        private IServiceProvider _provider;

        public IServiceProvider GetServiceProvider()
        {
            return _provider;
        }


        public void SetupDI(IServiceProvider provider)
        {
            _provider = provider;
        }
    }
}
