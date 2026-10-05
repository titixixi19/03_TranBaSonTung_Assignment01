using BackEnd.BusinessObjects;
using BackEnd.Common;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.DataAccess;

public sealed class TagDAO
{
    private static TagDAO? instance;
    private static readonly object instanceLock = new();

    private TagDAO() { }

    public static TagDAO Instance
    {
        get
        {
            lock (instanceLock)
            {
                instance ??= new TagDAO();
                return instance;
            }
        }
    }

    public List<Tag> GetTags()
    {
        using var context = new FUNewsManagementContext();
        return context.Tags.AsNoTracking().OrderBy(t => t.TagName).ToList();
    }

    public Tag? GetTagById(int id)
    {
        using var context = new FUNewsManagementContext();
        return context.Tags.AsNoTracking().FirstOrDefault(t => t.TagID == id);
    }

    private static void EnsureUniqueName(FUNewsManagementContext context, string name, int? excludeId = null)
    {
        var normalized = name.Trim().ToLower();
        if (context.Tags.Any(t => t.TagName!.Trim().ToLower() == normalized && (!excludeId.HasValue || t.TagID != excludeId)))
            throw new BusinessException("Tag name already exists.");
    }

    public Tag AddTag(Tag tag)
    {
        using var context = new FUNewsManagementContext();
        EnsureUniqueName(context, tag.TagName!);
        // TagID is not an identity column
        tag.TagID = (context.Tags.Max(t => (int?)t.TagID) ?? 0) + 1;
        context.Tags.Add(tag);
        context.SaveChanges();
        return tag;
    }

    public void UpdateTag(Tag tag)
    {
        using var context = new FUNewsManagementContext();
        var existing = context.Tags.FirstOrDefault(t => t.TagID == tag.TagID)
                       ?? throw new NotFoundException("Tag not found.");
        EnsureUniqueName(context, tag.TagName!, tag.TagID);

        existing.TagName = tag.TagName;
        existing.Note = tag.Note;
        context.SaveChanges();
    }

    public void DeleteTag(int id)
    {
        using var context = new FUNewsManagementContext();
        var tag = context.Tags.FirstOrDefault(t => t.TagID == id)
                  ?? throw new NotFoundException("Tag not found.");

        if (context.NewsArticles.Any(n => n.Tags.Any(t => t.TagID == id)))
            throw new BusinessException("This tag is used by news articles and cannot be deleted.");

        context.Tags.Remove(tag);
        context.SaveChanges();
    }
}
