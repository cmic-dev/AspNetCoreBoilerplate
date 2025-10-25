using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Modules.Auth.Core.Events;
using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Events;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.EventHandlers;

public class CreateUserProfileHandler(IAppDbContext dbContext) : IDomainEventHandler<UserCreatedEvent>
{
    private static readonly string[] Colors =
    [
        "3b82f6", // blue
        "ef4444", // red
        "10b981", // green
        "f59e0b", // amber
        "8b5cf6", // violet
        "ec4899", // pink
        "06b6d4", // cyan
        "f97316", // orange
        "6366f1", // indigo
        "14b8a6"  // teal
    ];

    public async Task HandleAsync(UserCreatedEvent domainEvent, IUserContext userContext, CancellationToken ctn = default)
    {
        var userProfile = UserProfile.Create(
            domainEvent.UserId,
            domainEvent.UserName);

        var randomColor = Colors[Random.Shared.Next(Colors.Length)];
        var profilePicture = $"https://ui-avatars.com/api/?name={domainEvent.UserName}&size=80&background={randomColor}&color=fff";

        userProfile.UpdateProfilePicture(profilePicture);
        dbContext.Set<UserProfile>().Add(userProfile);
        await dbContext.SaveChangesAsync(ctn);
    }
}
