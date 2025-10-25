using AspNetCoreBoilerplate.Shared.Authorization;
using AspNetCoreBoilerplate.Shared.Entities;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Entities;

public class Role : DomainEntity<Guid>
{
    public static readonly Role SuperAdmin = new Role()
    {
        Id = Guid.Parse("a1a2e5e2-95d3-4cb6-a56d-4f7e01c8921c"),
        Name = Roles.SuperAdmin,
        DisplayName = "Super Admin",
        Description = "System administrator with unrestricted access to all modules and settings.",
        IsSystem = true
    };

    public static readonly Role Admin = new Role
    {
        Id = Guid.Parse("b2b3f6f3-a6e4-5dc7-b67e-5f8f12d9a32d"),
        Name = Roles.Admin,
        DisplayName = "Admin",
        Description = "Administrator with broad access to system management and configuration.",
        IsSystem = true
    };

    public static readonly Role Moderator = new Role
    {
        Id = Guid.Parse("c3c4f7f4-b7f5-6ed8-c78f-6f9f13e0b43e"),
        Name = Roles.Moderator,
        DisplayName = "Moderator",
        Description = "Moderator with content moderation and user management capabilities.",
        IsSystem = true
    };

    public static readonly Role Manager = new Role
    {
        Id = Guid.Parse("d4d5f8f5-c8f6-7fe9-d89f-7f0f14f1c54f"),
        Name = Roles.Manager,
        DisplayName = "Manager",
        Description = "Manager with team and resource management responsibilities.",
        IsSystem = true
    };

    public static readonly Role Editor = new Role
    {
        Id = Guid.Parse("e5e6f9f6-d9f7-8af0-e9af-8f1f15f2d65f"),
        Name = Roles.Editor,
        DisplayName = "Editor",
        Description = "Editor with content creation and editing privileges.",
        IsSystem = true
    };

    public static readonly Role User = new Role
    {
        Id = Guid.Parse("f6f7f0f7-e0f8-9bf1-f0bf-9f2f16f3e76f"),
        Name = Roles.User,
        DisplayName = "User",
        Description = "Standard user with basic access to the application.",
        IsSystem = true
    };

    public static readonly Role Guest = new Role
    {
        Id = Guid.Parse("a7a8f1f8-f1f9-0cf2-a1cf-0f3f17f4f87f"),
        Name = Roles.Guest,
        DisplayName = "Guest",
        Description = "Guest user with limited access to public features only.",
        IsSystem = true
    };

    private Role() { }

    public static Role Create(Guid id, string name, string displayName, string description, bool isSystem)
    {
        return new Role()
        {
            Id = id,
            Name = name,
            DisplayName = displayName,
            Description = description,
            IsSystem = isSystem
        };
    }

    public string DisplayName { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; } = false;
}
