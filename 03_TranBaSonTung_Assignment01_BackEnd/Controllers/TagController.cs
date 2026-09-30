using BackEnd.BusinessObjects;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.StaffName)]
public class TagController : ControllerBase
{
    private readonly ITagRepository _repository;

    public TagController(ITagRepository repository) => _repository = repository;

    [HttpPost]
    public IActionResult Create([FromBody] TagRequest request)
    {
        var tag = _repository.AddTag(new Tag { TagName = request.TagName.Trim(), Note = request.Note?.Trim() });
        return Ok(new { tag.TagID, message = "Tag created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] TagRequest request)
    {
        _repository.UpdateTag(new Tag { TagID = id, TagName = request.TagName.Trim(), Note = request.Note?.Trim() });
        return Ok(new { message = "Tag updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _repository.DeleteTag(id);
        return Ok(new { message = "Tag deleted successfully." });
    }
}
