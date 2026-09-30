namespace FrontEnd.Models;

public class ODataResponse<T>
{
    public List<T> Value { get; set; } = new();
}
