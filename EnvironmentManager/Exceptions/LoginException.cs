namespace EnvironmentManager.Exceptions;

/// <summary>
/// Custom Exception for use in failed user authentication. 
/// 
/// Can be thrown with default message or provided a message in exception params.
/// </summary>
public class LoginException : Exception
{
    public LoginException() : base("Invalid Username or Password") {}

    public LoginException(string message) : base(message) {}

    public LoginException(string message, Exception inner) : base(message, inner) {}
}