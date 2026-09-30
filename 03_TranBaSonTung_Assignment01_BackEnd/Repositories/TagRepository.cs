using BackEnd.BusinessObjects;
using BackEnd.DataAccess;

namespace BackEnd.Repositories;

public class TagRepository : ITagRepository
{
    public List<Tag> GetTags() => TagDAO.Instance.GetTags();
    public Tag? GetTagById(int id) => TagDAO.Instance.GetTagById(id);
    public Tag AddTag(Tag tag) => TagDAO.Instance.AddTag(tag);
    public void UpdateTag(Tag tag) => TagDAO.Instance.UpdateTag(tag);
    public void DeleteTag(int id) => TagDAO.Instance.DeleteTag(id);
}
