using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace SharpSample;

[Dependency(ReplaceServices = true)]
public class SharpSampleBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "SharpSample";
}
