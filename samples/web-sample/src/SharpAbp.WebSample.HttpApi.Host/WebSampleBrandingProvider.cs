using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace SharpAbp.WebSample
{
    [Dependency(ReplaceServices = true)]
    public class WebSampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "WebSample";
    }
}
