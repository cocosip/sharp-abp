using DotCommon;
using FellowOakDicom;
using System;

namespace SharpAbp.FoDicom
{
    public class CustomServiceProviderHost : IServiceProviderHost
    {
        private IDotCommonApplication _application;

        public CustomServiceProviderHost(IDotCommonApplication application)
        {
            _application = application;
        }

        public IServiceProvider GetServiceProvider()
        {
            return _application.ServiceProvider;
        }


        //public void SetupDI(IServiceProvider provider)
        //{
        //    _provider = provider;
        //}
    }
}
