using BackEnd.BusinessObjects;
using BackEnd.Common;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.DataAccess;

public sealed class SystemAccountDAO
{
    private static SystemAccountDAO? instance;
    private static readonly object instanceLock = new();

    private SystemAccountDAO() { }

    public static SystemAccountDAO Instance
    {
        get
        {
            lock (instanceLock)
            {
                instance ??= new SystemAccountDAO();
                return instance;
            }
        }
    }

    public List<SystemAccount> GetAccounts()
    {
        using var context = new FUNewsManagementDbContext();
        return context.SystemAccounts.AsNoTracking().OrderBy(a => a.AccountID).ToList();
    }

    public SystemAccount? GetAccountById(short id)
    {
        using var context = new FUNewsManagementDbContext();
        return context.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountID == id);
    }

    public SystemAccount? Login(string email, string password)
    {
        using var context = new FUNewsManagementDbContext();
        // The database collation is case-insensitive, so only the email is matched in SQL;
        // the password is compared in memory with an exact (case-sensitive) comparison.
        var account = context.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountEmail == email);
        return account != null && string.Equals(account.AccountPassword, password, StringComparison.Ordinal)
            ? account
            : null;
    }

    public bool EmailExists(string email, short? excludeId = null)
    {
        using var context = new FUNewsManagementDbContext();
        return context.SystemAccounts.Any(a => a.AccountEmail == email && (!excludeId.HasValue || a.AccountID != excludeId));
    }

    public SystemAccount AddAccount(SystemAccount account)
    {
        using var context = new FUNewsManagementDbContext();
        // AccountID is not an identity column
        account.AccountID = (short)((context.SystemAccounts.Max(a => (short?)a.AccountID) ?? 0) + 1);
        context.SystemAccounts.Add(account);
        context.SaveChanges();
        return account;
    }

    public void UpdateAccount(SystemAccount account)
    {
        using var context = new FUNewsManagementDbContext();
        var existing = context.SystemAccounts.FirstOrDefault(a => a.AccountID == account.AccountID)
                       ?? throw new NotFoundException("Account not found.");

        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        existing.AccountRole = account.AccountRole;
        if (!string.IsNullOrEmpty(account.AccountPassword))
        {
            existing.AccountPassword = account.AccountPassword;
        }
        context.SaveChanges();
    }

    public void DeleteAccount(short id)
    {
        using var context = new FUNewsManagementDbContext();
        var account = context.SystemAccounts.FirstOrDefault(a => a.AccountID == id)
                      ?? throw new NotFoundException("Account not found.");

        if (context.NewsArticles.Any(n => n.CreatedByID == id))
            throw new BusinessException("This account has already created news articles and cannot be deleted.");

        context.SystemAccounts.Remove(account);
        context.SaveChanges();
    }
}
