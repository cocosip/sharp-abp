using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class RouteTranslationHeader
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Router { get; set; }
        public Dictionary<string, StringValues> Extends { get; protected set; }
        public RouteTranslationHeader()
        {
            Extends = [];
        }
    }
}
