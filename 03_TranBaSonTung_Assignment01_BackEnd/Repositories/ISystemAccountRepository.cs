using BackEnd.BusinessObjects;

namespace BackEnd.Repositories;

public interface ISystemAccountRepository
{
    List<SystemAccount> GetAccounts();
    SystemAccount? GetAccountById(short id);
    SystemAccount? Login(string email, string password);
    bool EmailExists(string email, short? excludeId = null);
    SystemAccount AddAccount(SystemAccount account);
    void UpdateAccount(SystemAccount account);
    void DeleteAccount(short id);
}
