using Volo.Abp.Settings;

namespace MainPlatform.Settings;

public class MainPlatformSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MainPlatformSettings.MySetting1));
    }
}
