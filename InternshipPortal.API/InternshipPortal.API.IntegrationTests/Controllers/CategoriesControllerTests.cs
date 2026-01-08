using System.Net;
using System.Net.Http.Json;
using Xunit;

using InternshipPortal.API.Data.EF;
using InternshipPortal.API.IntegrationTests.Infrastructure;

namespace InternshipPortal.API.IntegrationTests.Controllers
{
    public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CategoriesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk_AndSeededCategoryExists()
        {
            var res = await _client.GetAsync("/api/Categories");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var list = await res.Content.ReadFromJsonAsync<Category[]>();
            Assert.NotNull(list);
            Assert.Contains(list, c => c.Id == 1 && c.Name == "IT");
        }

        [Fact]
        public async Task GetById_Seeded_ReturnsOk()
        {
            var res = await _client.GetAsync("/api/Categories/1");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Fact]
        public async Task GetAll_SortMostUsed_ReturnsOk()
        {
            var res = await _client.GetAsync("/api/Categories?sort=mostUsed");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Fact]
        public async Task Create_Update_Delete_Works_ForNewCategory()
        {
            // CREATE
            var post = await _client.PostAsJsonAsync("/api/Categories", new Category { Name = "NewCat" });
            Assert.Equal(HttpStatusCode.Created, post.StatusCode);

            var created = await post.Content.ReadFromJsonAsync<Category>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            // UPDATE
            var put = await _client.PutAsJsonAsync($"/api/Categories/{created.Id}", new Category { Name = "NewCat2" });
            Assert.Equal(HttpStatusCode.OK, put.StatusCode);

            // DELETE 
            var del = await _client.DeleteAsync($"/api/Categories/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
        }

        [Fact]
        public async Task Delete_SeededCategoryWithInternships_ReturnsBadRequest()
        {
            var del = await _client.DeleteAsync("/api/Categories/1");
            Assert.Equal(HttpStatusCode.BadRequest, del.StatusCode);
        }
    }
}
