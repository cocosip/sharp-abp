using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace IdentitySample
{
    [Dependency(ReplaceServices = true)]
    public class IdentitySampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "IdentitySample";
    }
}
