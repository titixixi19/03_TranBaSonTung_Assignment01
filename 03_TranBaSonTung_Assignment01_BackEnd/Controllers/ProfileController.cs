using BackEnd.BusinessObjects;
using BackEnd.Common;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.StaffName)]
public class ProfileController : ControllerBase
{
    private readonly ISystemAccountRepository _repository;
    private readonly IConfiguration _configuration;

    public ProfileController(ISystemAccountRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var account = _repository.GetAccountById(User.GetAccountId());
        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }
        return Ok(new { account.AccountID, account.AccountName, account.AccountEmail, account.AccountRole });
    }

    [HttpPut]
    public IActionResult Update([FromBody] ProfileUpdateRequest request)
    {
        var id = User.GetAccountId();
        var account = _repository.GetAccountById(id);
        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        var isAdminEmail = string.Equals(request.AccountEmail, _configuration["AdminAccount:Email"], StringComparison.OrdinalIgnoreCase);
        if (isAdminEmail || _repository.EmailExists(request.AccountEmail, id))
        {
            return BadRequest(new { message = "Email already exists." });
        }

        _repository.UpdateAccount(new SystemAccount
        {
            AccountID = id,
            AccountName = request.AccountName.Trim(),
            AccountEmail = request.AccountEmail.Trim(),
            AccountRole = account.AccountRole,
            AccountPassword = request.AccountPassword
        });
        return Ok(new { message = "Profile updated successfully." });
    }
}
