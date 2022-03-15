using Volo.Abp.Settings;

namespace AllApiSample.Settings;

public class AllApiSampleSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AllApiSampleSettings.MySetting1));
    }
}
