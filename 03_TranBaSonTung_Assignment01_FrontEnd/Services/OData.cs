namespace FrontEnd.Services;

public static class OData
{
    // Builds a lower-cased, quote-escaped OData string literal
    public static string Literal(string value) => "'" + value.Trim().ToLower().Replace("'", "''") + "'";

    public static string Escape(string value) => Uri.EscapeDataString(value);
}
