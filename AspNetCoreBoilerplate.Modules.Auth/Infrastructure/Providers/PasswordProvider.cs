using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Providers;

internal class PasswordProvider : IPasswordProvider
{
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
