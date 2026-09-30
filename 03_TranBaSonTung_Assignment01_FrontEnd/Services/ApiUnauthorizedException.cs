namespace FrontEnd.Services;

public class ApiUnauthorizedException : Exception
{
    public ApiUnauthorizedException(string message) : base(message) { }
}
