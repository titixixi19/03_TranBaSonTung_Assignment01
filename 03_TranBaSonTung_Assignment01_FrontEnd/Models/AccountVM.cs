namespace FrontEnd.Models;

public class AccountVM
{
    public short AccountID { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }

    public string RoleName => AccountRole switch
    {
        1 => "Staff",
        2 => "Lecturer",
        _ => "Unknown"
    };
}
