using System.Net;
using System.Net.Http.Headers;

namespace Abjjad.Images.Tests.Integration;

public class ImagesControllerIntegrationTests
{
    private readonly HttpClient _client;

    public ImagesControllerIntegrationTests()
    {
        var factory = new AbjjadImagesWebApplicationFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_ValidImages_ReturnsOk()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(new byte[1024]); // 1KB dummy image
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(imageContent, "Images", "test.jpg");

        // Act
        var response = await _client.PostAsync("/api/Images", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMetadata_InvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/Images/00000000-0000-0000-0000-000000000000/metadata");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}