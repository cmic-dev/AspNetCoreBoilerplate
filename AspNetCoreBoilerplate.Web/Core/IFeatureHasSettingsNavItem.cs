using AspNetCoreBoilerplate.Web.Core.Models;

namespace AspNetCoreBoilerplate.Web.Core;

public interface IFeatureHasSettingsNavItem
{
    public IEnumerable<SettingsNavItem> GetNavItems();
}
