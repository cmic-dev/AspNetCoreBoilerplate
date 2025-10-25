namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IPasswordProvider
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
