using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Collaboration.Blazor.Menus;

public class CollaborationMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(CollaborationMenus.Prefix, displayName: "Collaboration", "/Collaboration", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
