using BackEnd.BusinessObjects;

namespace BackEnd.Repositories;

public interface ITagRepository
{
    List<Tag> GetTags();
    Tag? GetTagById(int id);
    Tag AddTag(Tag tag);
    void UpdateTag(Tag tag);
    void DeleteTag(int id);
}
