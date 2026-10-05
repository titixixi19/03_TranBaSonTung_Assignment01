using BackEnd.BusinessObjects;
using BackEnd.Common;
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

    // Ensures the parent exists and that the new parent does not create a loop (A -> B -> A)
    private static void ValidateParent(FUNewsManagementContext context, short? parentId, short? categoryId = null)
    {
        if (!parentId.HasValue) return;

        if (parentId == categoryId)
            throw new BusinessException("A category cannot be its own parent.");

        var parents = context.Categories.AsNoTracking()
            .Select(c => new { c.CategoryID, c.ParentCategoryID })
            .ToDictionary(c => c.CategoryID, c => c.ParentCategoryID);

        if (!parents.ContainsKey(parentId.Value))
            throw new BusinessException("Parent category does not exist.");

        if (!categoryId.HasValue) return;

        // Walk up from the new parent; reaching the category itself means a loop
        var visited = new HashSet<short>();
        short? current = parentId;
        while (current.HasValue && visited.Add(current.Value) && parents.TryGetValue(current.Value, out var next))
        {
            if (current == categoryId)
                throw new BusinessException("The selected parent would create a circular category hierarchy.");
            current = next;
        }
    }

    public Category AddCategory(Category category)
    {
        using var context = new FUNewsManagementContext();
        ValidateParent(context, category.ParentCategoryID);

        context.Categories.Add(category);
        context.SaveChanges();
        return category;
    }

    public void UpdateCategory(Category category)
    {
        using var context = new FUNewsManagementContext();
        var existing = context.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID)
                       ?? throw new NotFoundException("Category not found.");

        ValidateParent(context, category.ParentCategoryID, category.CategoryID);

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
                       ?? throw new NotFoundException("Category not found.");

        if (context.NewsArticles.Any(n => n.CategoryID == id))
            throw new BusinessException("This category already belongs to news articles and cannot be deleted.");

        if (context.Categories.Any(c => c.ParentCategoryID == id && c.CategoryID != id))
            throw new BusinessException("This category is the parent of other categories and cannot be deleted.");

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
