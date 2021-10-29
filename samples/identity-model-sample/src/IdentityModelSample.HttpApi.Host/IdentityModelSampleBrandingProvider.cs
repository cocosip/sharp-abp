using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace IdentityModelSample
{
    [Dependency(ReplaceServices = true)]
    public class IdentityModelSampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "IdentityModelSample";
    }
}
