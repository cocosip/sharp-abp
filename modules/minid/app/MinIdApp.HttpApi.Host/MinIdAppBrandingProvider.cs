using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MinIdApp
{
    [Dependency(ReplaceServices = true)]
    public class MinIdAppBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MinIdApp";
    }
}
