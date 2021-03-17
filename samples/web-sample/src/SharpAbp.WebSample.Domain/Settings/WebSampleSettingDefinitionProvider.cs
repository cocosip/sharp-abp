using Volo.Abp.Settings;

namespace SharpAbp.WebSample.Settings
{
    public class WebSampleSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(WebSampleSettings.MySetting1));
        }
    }
}
