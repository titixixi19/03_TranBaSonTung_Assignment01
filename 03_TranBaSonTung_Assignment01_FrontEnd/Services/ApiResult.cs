using System.Text.Json;

namespace FrontEnd.Services;

public class ApiResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public JsonElement? Data { get; set; }
}
