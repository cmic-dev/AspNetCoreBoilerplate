namespace AspNetCoreBoilerplate.Web.Core.Models;

public class Link
{
    public string Title { get; set; }
    public string Href { get; set; }
    public string Icon { get; set; }
}

public class SettingsNavItem : Link 
{ 
    public SettingsNavItem(string title,
        string href,
        string icon,
        IEnumerable<string>? requiredRoles = default)
    {
        Title = title;
        Href = href;
        Icon = icon;
        RequiredRoles = requiredRoles;
    }

    public IEnumerable<string>? RequiredRoles { get; set; }

    public bool IsVisible(string userRole)
    {
        if (RequiredRoles == null)
            return true;
        return RequiredRoles.Any(x => x == userRole);
    }
}

public class NavBarItem : Link
{
    public NavBarItem(string title,
        string href,
        string icon,
        IEnumerable<string>? requiredRoles = default,
        IEnumerable<NavBarItem>? children = default)
    {
        Title = title;
        Href = href;
        Icon = icon;
        RequiredRoles = requiredRoles;
        Children = children;
    }

    public IEnumerable<string>? RequiredRoles { get; set; }
    public IEnumerable<NavBarItem>? Children { get; set; } = null;

    public bool IsVisible(string userRole)
    {
        if (RequiredRoles == null)
            return true;
        return RequiredRoles.Any(x => x == userRole);
    }
}
