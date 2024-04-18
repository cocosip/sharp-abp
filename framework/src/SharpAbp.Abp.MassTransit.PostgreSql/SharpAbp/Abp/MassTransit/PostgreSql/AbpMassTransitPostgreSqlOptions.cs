using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class AbpMassTransitPostgreSqlOptions
    {
        public AbpMassTransitPostgreSqlOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitPostgreSqlOptions = configuration
                .GetSection("MassTransitOptions:PostgreSqlOptions")
                .Get<AbpMassTransitPostgreSqlOptions>();

            if (massTransitPostgreSqlOptions != null)
            {
                

            }

            return this;
        }
    }
}
