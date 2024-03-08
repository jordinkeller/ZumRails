using Test.Entities;

namespace Test.DTOs
{
    public class PostsResponse
    {
        public IEnumerable<Post> posts { get; set; }
    }
}
