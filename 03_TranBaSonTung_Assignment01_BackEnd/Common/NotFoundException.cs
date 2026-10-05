namespace BackEnd.Common;

// Thrown when the requested record does not exist (returned to the client as 404 Not Found)
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
