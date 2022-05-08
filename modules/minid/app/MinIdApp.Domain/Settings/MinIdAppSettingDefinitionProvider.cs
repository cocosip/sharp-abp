using Volo.Abp.Settings;

namespace MinIdApp.Settings
{
    public class MinIdAppSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MinIdAppSettings.MySetting1));
        }
    }
}
