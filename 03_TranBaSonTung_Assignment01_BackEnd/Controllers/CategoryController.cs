using BackEnd.BusinessObjects;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.StaffName)]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repository;

    public CategoryController(ICategoryRepository repository) => _repository = repository;

    [HttpPost]
    public IActionResult Create([FromBody] CategoryRequest request)
    {
        var category = _repository.AddCategory(new Category
        {
            CategoryName = request.CategoryName.Trim(),
            CategoryDesciption = request.CategoryDesciption.Trim(),
            ParentCategoryID = request.ParentCategoryID,
            IsActive = request.IsActive
        });
        return Ok(new { category.CategoryID, message = "Category created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(short id, [FromBody] CategoryRequest request)
    {
        _repository.UpdateCategory(new Category
        {
            CategoryID = id,
            CategoryName = request.CategoryName.Trim(),
            CategoryDesciption = request.CategoryDesciption.Trim(),
            ParentCategoryID = request.ParentCategoryID,
            IsActive = request.IsActive
        });
        return Ok(new { message = "Category updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(short id)
    {
        _repository.DeleteCategory(id);
        return Ok(new { message = "Category deleted successfully." });
    }
}
