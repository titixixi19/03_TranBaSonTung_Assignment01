using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services;

// Thin wrapper around HttpClient that attaches the JWT stored in session
public class ApiClient
{
    public static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string url, object? body)
    {
        var request = new HttpRequestMessage(method, url);
        var token = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.Token);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (body != null)
        {
            request.Content = JsonContent.Create(body);
        }
        return request;
    }

    private static void ThrowIfUnauthorized(HttpResponseMessage response)
    {
        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new ApiUnauthorizedException("Your session has expired. Please log in again.");
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        using var response = await _http.SendAsync(BuildRequest(HttpMethod.Get, url, null));
        if (response.StatusCode == HttpStatusCode.NotFound) return default;
        ThrowIfUnauthorized(response);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<List<T>> GetODataListAsync<T>(string url) =>
        (await GetAsync<ODataResponse<T>>(url))?.Value ?? new List<T>();

    public Task<ApiResult> PostAsync(string url, object body) => SendAsync(HttpMethod.Post, url, body);
    public Task<ApiResult> PutAsync(string url, object body) => SendAsync(HttpMethod.Put, url, body);
    public Task<ApiResult> DeleteAsync(string url) => SendAsync(HttpMethod.Delete, url, null);

    // Used by login: returns the error message instead of throwing on 401
    public async Task<ApiResult> SendAsync(HttpMethod method, string url, object? body, bool throwOnUnauthorized = true)
    {
        using var response = await _http.SendAsync(BuildRequest(method, url, body));
        if (throwOnUnauthorized) ThrowIfUnauthorized(response);

        var content = await response.Content.ReadAsStringAsync();
        JsonElement? json = null;
        if (!string.IsNullOrWhiteSpace(content))
        {
            try { json = JsonDocument.Parse(content).RootElement.Clone(); } catch (JsonException) { }
        }

        return new ApiResult
        {
            Success = response.IsSuccessStatusCode,
            Data = json,
            Message = ExtractMessage(json) ?? (response.IsSuccessStatusCode
                ? "Success."
                : response.StatusCode == HttpStatusCode.Forbidden
                    ? "You do not have permission to perform this action."
                    : $"Request failed ({(int)response.StatusCode}).")
        };
    }

    private static string? ExtractMessage(JsonElement? json)
    {
        if (json is not { ValueKind: JsonValueKind.Object } element) return null;

        if (element.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
            return message.GetString();

        if (element.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
        {
            var list = errors.EnumerateObject()
                .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                .Where(s => !string.IsNullOrEmpty(s));
            return string.Join(" ", list);
        }
        return null;
    }
}
