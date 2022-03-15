using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AllApiSample;

[Dependency(ReplaceServices = true)]
public class AllApiSampleBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "AllApiSample";
}
