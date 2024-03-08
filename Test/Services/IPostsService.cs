using Test.Entities;

namespace Test.Services
{
    public interface IPostsService
    {
        Task<IEnumerable<Post>> GetPosts(string tag);
    }
}
