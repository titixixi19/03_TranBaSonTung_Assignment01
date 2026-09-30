using BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BackEnd.Controllers.OData;

public class CategoriesController : ODataController
{
    private readonly ICategoryRepository _repository;

    public CategoriesController(ICategoryRepository repository) => _repository = repository;

    [EnableQuery]
    public IActionResult Get() => Ok(_repository.GetCategories().AsQueryable());

    [EnableQuery]
    public IActionResult Get([FromRoute] short key)
    {
        var category = _repository.GetCategoryById(key);
        return category == null ? NotFound() : Ok(category);
    }
}
