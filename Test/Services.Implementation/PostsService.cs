using System.Collections;
using System.Net;
using System.Text.Json;
using Test.DTOs;
using Test.Entities;

namespace Test.Services.Implementation
{
    public class PostsService : IPostsService
    {
        private readonly HttpClient _httpClient;
        public PostsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Post>> GetPosts(string tag)
        {
            var response = await _httpClient.GetAsync($"https://api.hatchways.io/assessment/blog/posts?tag={tag}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var responseObject = await JsonSerializer.DeserializeAsync<PostsResponse>(content);
                if (responseObject == null || responseObject.posts == null)
                {
                    throw new Exception("Failed to retrieve posts from API response");
                }
                else
                {
                    return responseObject.posts;
                }
            }
            else
            {
                throw new HttpRequestException($"{response.StatusCode} : {response.Content}");
            }
        }
    }
}
