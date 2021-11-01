using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace FileStoringSample
{
    [Dependency(ReplaceServices = true)]
    public class FileStoringSampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "FileStoringSample";
    }
}
