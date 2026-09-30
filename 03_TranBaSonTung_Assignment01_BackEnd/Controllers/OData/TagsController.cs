using BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BackEnd.Controllers.OData;

public class TagsController : ODataController
{
    private readonly ITagRepository _repository;

    public TagsController(ITagRepository repository) => _repository = repository;

    [EnableQuery]
    public IActionResult Get() => Ok(_repository.GetTags().AsQueryable());

    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var tag = _repository.GetTagById(key);
        return tag == null ? NotFound() : Ok(tag);
    }
}
