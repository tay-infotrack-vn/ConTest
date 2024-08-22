using FluentAssertions;

namespace ProductTest.Tests
{
    public class ProductTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public ProductTests(CustomWebApplicationFactory<Program> factory) 
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test()
        {
            var response = await _client.GetAsync("/product");
            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be("Value not in cache");
        }
    }
}
