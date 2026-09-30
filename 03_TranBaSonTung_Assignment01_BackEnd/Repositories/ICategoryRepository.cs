using BackEnd.BusinessObjects;

namespace BackEnd.Repositories;

public interface ICategoryRepository
{
    List<Category> GetCategories();
    Category? GetCategoryById(short id);
    Category AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(short id);
}
