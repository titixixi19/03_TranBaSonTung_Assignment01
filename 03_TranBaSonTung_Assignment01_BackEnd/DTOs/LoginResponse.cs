namespace BackEnd.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public short AccountID { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
