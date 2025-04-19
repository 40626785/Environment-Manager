namespace EnvironmentManager.Exceptions;

//Custom exception for unsuccessful login
public class LoginException : Exception
{
    public LoginException() : base("Invalid Username or Password") {}

    public LoginException(string message) : base(message) {}

    public LoginException(string message, Exception inner) : base(message, inner) {}
}