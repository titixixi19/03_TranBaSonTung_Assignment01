using BackEnd.BusinessObjects;
using BackEnd.DataAccess;

namespace BackEnd.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public List<Category> GetCategories() => CategoryDAO.Instance.GetCategories();
    public Category? GetCategoryById(short id) => CategoryDAO.Instance.GetCategoryById(id);
    public Category AddCategory(Category category) => CategoryDAO.Instance.AddCategory(category);
    public void UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);
    public void DeleteCategory(short id) => CategoryDAO.Instance.DeleteCategory(id);
}
