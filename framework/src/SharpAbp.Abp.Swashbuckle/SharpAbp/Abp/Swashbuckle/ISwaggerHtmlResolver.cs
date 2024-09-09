using System.IO;

namespace SharpAbp.Abp.Swashbuckle
{
    public interface ISwaggerHtmlResolver
    {
        Stream Resolver();
    }
}
