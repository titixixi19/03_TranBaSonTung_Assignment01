namespace FrontEnd.Services;

// The API could not be reached or returned an unexpected error while loading data
public class ApiException : Exception
{
    public ApiException(string message) : base(message) { }
}
