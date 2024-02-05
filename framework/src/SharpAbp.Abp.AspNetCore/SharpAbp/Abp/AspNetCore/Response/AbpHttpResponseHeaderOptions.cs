using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.Response
{
    public class AbpHttpResponseHeaderOptions
    {
        public Dictionary<string, StringValues> Headers { get; }

        public AbpHttpResponseHeaderOptions()
        {
            Headers = [];
        }

        public AbpHttpResponseHeaderOptions Configure(IConfiguration configuration)
        {
            var headers = configuration
                .GetSection("ResponseHeaderOptions")
                .Get<Dictionary<string, string[]>>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    Headers.Add(header.Key, new StringValues(header.Value));
                }
            }
            return this;
        }
    }
}
