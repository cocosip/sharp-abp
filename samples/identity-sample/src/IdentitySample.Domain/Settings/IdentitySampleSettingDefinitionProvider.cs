using Volo.Abp.Settings;

namespace IdentitySample.Settings
{
    public class IdentitySampleSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(IdentitySampleSettings.MySetting1));
        }
    }
}
