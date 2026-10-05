namespace BackEnd.Common;

// Thrown when a business rule is violated (returned to the client as 400 Bad Request)
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
