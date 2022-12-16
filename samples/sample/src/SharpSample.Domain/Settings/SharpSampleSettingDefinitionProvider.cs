using Volo.Abp.Settings;

namespace SharpSample.Settings;

public class SharpSampleSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(SharpSampleSettings.MySetting1));
    }
}
