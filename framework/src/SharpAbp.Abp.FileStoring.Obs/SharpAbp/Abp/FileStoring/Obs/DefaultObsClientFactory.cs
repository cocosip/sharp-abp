using OBS;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class DefaultObsClientFactory : IObsClientFactory, ITransientDependency
    {

        public virtual ObsClient Create(ObsFileProviderConfiguration configuration)
        {
            var config = new ObsConfig()
            {
            };
            var client = new ObsClient("", "", "",new ObsConfig())
            {

            };
            return client;
        }

    }
}
