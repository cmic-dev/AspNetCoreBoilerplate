using AspNetCoreBoilerplate.Web.Core.Models;

namespace AspNetCoreBoilerplate.Web.Core;

public interface IFeatureHasNavItem
{
    public IEnumerable<NavBarItem> GetNavItems();
}
