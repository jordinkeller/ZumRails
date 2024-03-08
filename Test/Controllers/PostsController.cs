using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using Test.Entities;
using Test.Services;
using Test.Services.Implementation;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {

        private readonly IPostsService _postsService;
        public PostsController(IPostsService postsService) 
        {
            _postsService = postsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsByTag(string tags, string sortBy, string direction = "asc") 
        {
            try
            {
                ValidateInputs(tags, sortBy, direction);

                var tagsToSearch = tags.Split(',');

                var tasks = tagsToSearch.Select(tag => _postsService.GetPosts(tag));
                var responses = await Task.WhenAll(tasks);

                var postList = new List<Post>();
                foreach (var response in responses)
                {
                    if (response != null)
                    {
                        postList.AddRange(response);
                    }
                }
                postList = SortPosts(postList.DistinctBy(x => x.id), sortBy, direction).ToList();
                return Ok(postList);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        private bool SortByIsValid(string sortBy)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var sortByLowerCase = sortBy.ToLower();
                return sortByLowerCase == "id"
                    || sortByLowerCase == "reads"
                    || sortByLowerCase == "likes"
                    || sortByLowerCase == "popularity"
                    ;
            }
            return false;
        }

        private bool DirectionIsValid(string direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                var directionLowerCase = direction.ToLower();
                return directionLowerCase == "asc"
                    || directionLowerCase == "desc";
            }
            return false;
        }

        private void ValidateInputs(string tags, string sortBy, string direction)
        {
            if (string.IsNullOrEmpty(tags))
            {
                throw new ArgumentException("tags parameter is required");
            }

            if (!SortByIsValid(sortBy))
            {
                throw new ArgumentException("sortBy parameter is invalid");
            }

            if (!DirectionIsValid(direction))
            {
                throw new ArgumentException("direction parameter is invalid");
            }
        }

        private IEnumerable<Post> SortPosts(IEnumerable<Post> postList, string sortBy, string direction)
        {
            bool ascending = direction.ToLower() == "asc";
            switch (sortBy.ToLower())
            {
                case "id":
                    return postList.OrderBy(x => x.id);
                case "reads":
                    return postList.OrderBy(x => x.reads);
                case "likes":
                    return postList.OrderBy(x => x.likes);
                case "popularity":
                    return postList.OrderBy(x => x.popularity);
                default:
                    return ascending ? postList.ToList() : postList.OrderByDescending(x => x.id).ToList();
            }
        }
    }
}
