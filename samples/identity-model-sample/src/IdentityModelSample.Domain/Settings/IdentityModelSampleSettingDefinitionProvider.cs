using Volo.Abp.Settings;

namespace IdentityModelSample.Settings
{
    public class IdentityModelSampleSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(IdentityModelSampleSettings.MySetting1));
        }
    }
}
