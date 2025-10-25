using AspNetCoreBoilerplate.Shared.Entities;
using AspNetCoreBoilerplate.Shared.Exceptions;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Entities;

public class UserProfile : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public string FullName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? Gender { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(Guid userId, string fullName, string? gender = null, string? phoneNumber = null, DateOnly? dateOfBirth = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty");

        var userProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName.Trim(),
            Gender = gender?.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            DateOfBirth = dateOfBirth
        };

        return userProfile;
    }

    public void UpdateProfile(string? fullName, string? gender, string? phoneNumber, DateOnly? dateOfBirth)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            if (fullName.Length > 500)
                throw new ArgumentException("Full name cannot exceed 500 characters", nameof(fullName));
            FullName = fullName.Trim();
        }

        Gender = gender?.Trim();
        PhoneNumber = phoneNumber?.Trim();

        if (dateOfBirth.HasValue)
        {
            ValidateDateOfBirth(dateOfBirth.Value);
            DateOfBirth = dateOfBirth;
        }
    }

    public void UpdateProfilePicture(string pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
            throw new ArgumentException("Picture URL cannot be empty", nameof(pictureUrl));

        if (pictureUrl.Length > 2048)
            throw new ArgumentException("Picture URL cannot exceed 2048 characters", nameof(pictureUrl));

        ProfilePictureUrl = pictureUrl.Trim();
    }

    public void ClearProfilePicture()
    {
        ProfilePictureUrl = null;
    }

    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        if (fullName.Length > 500)
            throw new ArgumentException("Full name cannot exceed 500 characters", nameof(fullName));

        FullName = fullName.Trim();
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber?.Trim();
    }

    public void UpdateGender(string? gender)
    {
        Gender = gender?.Trim();
    }

    public void UpdateDateOfBirth(DateOnly? dateOfBirth)
    {
        if (dateOfBirth.HasValue)
        {
            ValidateDateOfBirth(dateOfBirth.Value);
        }

        DateOfBirth = dateOfBirth;
    }

    public int? GetAge()
    {
        if (!DateOfBirth.HasValue)
            return null;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - DateOfBirth.Value.Year;

        if (DateOfBirth.Value > today.AddYears(-age))
            age--;

        return age >= 0 ? age : null;
    }

    private static void ValidateDateOfBirth(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (dateOfBirth > today)
            throw new DomainException("Date of birth cannot be in the future");

        var age = today.Year - dateOfBirth.Year;
        if (age > 150)
            throw new DomainException("Invalid date of birth");
    }
}
