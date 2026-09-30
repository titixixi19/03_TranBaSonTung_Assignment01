using BackEnd.BusinessObjects;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.AdminName)]
public class AccountController : ControllerBase
{
    private readonly ISystemAccountRepository _repository;
    private readonly IConfiguration _configuration;

    public AccountController(ISystemAccountRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    private bool IsAdminEmail(string email) =>
        string.Equals(email, _configuration["AdminAccount:Email"], StringComparison.OrdinalIgnoreCase);

    [HttpPost]
    public IActionResult Create([FromBody] AccountCreateRequest request)
    {
        if (IsAdminEmail(request.AccountEmail) || _repository.EmailExists(request.AccountEmail))
        {
            return BadRequest(new { message = "Email already exists." });
        }

        var account = _repository.AddAccount(new SystemAccount
        {
            AccountName = request.AccountName.Trim(),
            AccountEmail = request.AccountEmail.Trim(),
            AccountRole = request.AccountRole,
            AccountPassword = request.AccountPassword
        });
        return Ok(new { account.AccountID, message = "Account created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(short id, [FromBody] AccountUpdateRequest request)
    {
        if (_repository.GetAccountById(id) == null)
        {
            return NotFound(new { message = "Account not found." });
        }
        if (IsAdminEmail(request.AccountEmail) || _repository.EmailExists(request.AccountEmail, id))
        {
            return BadRequest(new { message = "Email already exists." });
        }

        _repository.UpdateAccount(new SystemAccount
        {
            AccountID = id,
            AccountName = request.AccountName.Trim(),
            AccountEmail = request.AccountEmail.Trim(),
            AccountRole = request.AccountRole,
            AccountPassword = request.AccountPassword
        });
        return Ok(new { message = "Account updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(short id)
    {
        _repository.DeleteAccount(id);
        return Ok(new { message = "Account deleted successfully." });
    }
}
