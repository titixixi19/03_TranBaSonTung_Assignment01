using BackEnd.BusinessObjects;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BackEnd.Controllers.OData;

[Authorize(Roles = AccountRoles.AdminName)]
public class SystemAccountsController : ODataController
{
    private readonly ISystemAccountRepository _repository;

    public SystemAccountsController(ISystemAccountRepository repository) => _repository = repository;

    [EnableQuery]
    public IActionResult Get() => Ok(_repository.GetAccounts().AsQueryable());

    [EnableQuery]
    public IActionResult Get([FromRoute] short key)
    {
        var account = _repository.GetAccountById(key);
        return account == null ? NotFound() : Ok(account);
    }
}
