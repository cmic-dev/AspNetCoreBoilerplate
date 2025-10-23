using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class HttpRequestExceptionExtensions
{
    public static string ToHumanReadableLoginErrorMessage(this HttpRequestException exception)
    {
        return exception.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Please check your username and password.",
            HttpStatusCode.Unauthorized => "Invalid username or password.",
            HttpStatusCode.Forbidden => "Your account has been disabled.",
            HttpStatusCode.TooManyRequests => "Too many login attempts. Please try again later.",
            HttpStatusCode.InternalServerError => "Server error. Please try again later.",
            HttpStatusCode.BadGateway => "Connection error. Please try again later.",
            HttpStatusCode.ServiceUnavailable => "Service unavailable. Please try again later.",
            HttpStatusCode.GatewayTimeout => "Request timed out. Please try again.",
            HttpStatusCode.Locked => "Your account is locked. Please try again later.",
            _ => "Login failed. Please try again."
        };
    }

    public static string ToHumanReadableErrorMessage(this HttpRequestException exception)
    {
        return exception switch
        {
            // HTTP Status Code Errors
            { StatusCode: HttpStatusCode.BadRequest } =>
                "Invalid request. Please check your input and try again.",

            { StatusCode: HttpStatusCode.Unauthorized } =>
                "Authentication failed. Please check your credentials.",

            { StatusCode: HttpStatusCode.Forbidden } =>
                "You don't have permission to access this resource.",

            { StatusCode: HttpStatusCode.NotFound } =>
                "The requested resource was not found.",

            { StatusCode: HttpStatusCode.Conflict } =>
                "A conflict occurred. The resource may have been modified.",

            { StatusCode: HttpStatusCode.UnprocessableEntity } =>
                "Unable to process your request. Please check your input.",

            { StatusCode: HttpStatusCode.TooManyRequests } =>
                "Too many requests. Please wait a moment and try again.",

            { StatusCode: HttpStatusCode.InternalServerError } =>
                "Server error occurred. Please try again later.",

            { StatusCode: HttpStatusCode.BadGateway } =>
                "Server communication error. Please try again later.",

            { StatusCode: HttpStatusCode.ServiceUnavailable } =>
                "Service temporarily unavailable. Please try again later.",

            { StatusCode: HttpStatusCode.GatewayTimeout } =>
                "Request timed out. Please try again.",

            // Network/Socket Errors
            { InnerException: SocketException socketEx } => socketEx.SocketErrorCode switch
            {
                SocketError.HostNotFound =>
                    "Unable to connect to the server. Please check your internet connection.",

                SocketError.TimedOut =>
                    "Connection timed out. Please try again.",

                SocketError.ConnectionRefused =>
                    "Connection refused. The server may be down.",

                SocketError.NetworkUnreachable =>
                    "Network is unreachable. Please check your internet connection.",

                SocketError.ConnectionReset =>
                    "Connection was reset. Please try again.",

                SocketError.HostUnreachable =>
                    "Server is unreachable. Please check your connection.",

                _ =>
                    "Network error occurred. Please check your connection and try again."
            },

            // Timeout Errors
            { InnerException: TaskCanceledException } =>
                "Request timed out. Please try again.",

            { InnerException: TimeoutException } =>
                "Request took too long to complete. Please try again.",

            { InnerException: OperationCanceledException } =>
                "Operation was cancelled. Please try again.",

            // SSL/Certificate Errors
            { InnerException: AuthenticationException } =>
                "Secure connection failed. Please check your network settings.",

            { InnerException: HttpRequestException { InnerException: AuthenticationException } } =>
                "Certificate validation failed. Please check your security settings.",

            // Generic Fallback
            _ => "An unexpected error occurred. Please try again."
        };
    }
}
