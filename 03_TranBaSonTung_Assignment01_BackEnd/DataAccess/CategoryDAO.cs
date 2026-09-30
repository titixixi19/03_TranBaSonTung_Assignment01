using BackEnd.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.DataAccess;

public sealed class CategoryDAO
{
    private static CategoryDAO? instance;
    private static readonly object instanceLock = new();

    private CategoryDAO() { }

    public static CategoryDAO Instance
    {
        get
        {
            lock (instanceLock)
            {
                instance ??= new CategoryDAO();
                return instance;
            }
        }
    }

    public List<Category> GetCategories()
    {
        using var context = new FUNewsManagementContext();
        return context.Categories
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .OrderBy(c => c.CategoryID)
            .ToList();
    }

    public Category? GetCategoryById(short id)
    {
        using var context = new FUNewsManagementContext();
        return context.Categories
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .FirstOrDefault(c => c.CategoryID == id);
    }

    public Category AddCategory(Category category)
    {
        using var context = new FUNewsManagementContext();
        if (category.ParentCategoryID.HasValue && !context.Categories.Any(c => c.CategoryID == category.ParentCategoryID))
            throw new InvalidOperationException("Parent category does not exist.");

        context.Categories.Add(category);
        context.SaveChanges();
        return category;
    }

    public void UpdateCategory(Category category)
    {
        using var context = new FUNewsManagementContext();
        var existing = context.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID)
                       ?? throw new KeyNotFoundException("Category not found.");

        if (category.ParentCategoryID.HasValue && !context.Categories.Any(c => c.CategoryID == category.ParentCategoryID))
            throw new InvalidOperationException("Parent category does not exist.");

        existing.CategoryName = category.CategoryName;
        existing.CategoryDesciption = category.CategoryDesciption;
        existing.ParentCategoryID = category.ParentCategoryID;
        existing.IsActive = category.IsActive;
        context.SaveChanges();
    }

    public void DeleteCategory(short id)
    {
        using var context = new FUNewsManagementContext();
        var category = context.Categories.FirstOrDefault(c => c.CategoryID == id)
                       ?? throw new KeyNotFoundException("Category not found.");

        if (context.NewsArticles.Any(n => n.CategoryID == id))
            throw new InvalidOperationException("This category already belongs to news articles and cannot be deleted.");

        if (context.Categories.Any(c => c.ParentCategoryID == id && c.CategoryID != id))
            throw new InvalidOperationException("This category is the parent of other categories and cannot be deleted.");

        // Seed data uses self-referencing parents; clear it before deleting
        if (category.ParentCategoryID == id)
        {
            category.ParentCategoryID = null;
            context.SaveChanges();
        }

        context.Categories.Remove(category);
        context.SaveChanges();
    }
}
