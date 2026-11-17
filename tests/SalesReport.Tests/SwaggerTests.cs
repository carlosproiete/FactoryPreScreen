using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SalesReport.Tests;

public class SwaggerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_Root_RedirectsToSwagger()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/swagger", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Get_SwaggerUi_ReturnsAnHtmlPage()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Get_OpenApiDocument_DescribesTheSalesReportEndpoint()
    {
        var client = factory.CreateClient();

        using var json = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json"));

        var responses = json.RootElement.GetProperty("paths").GetProperty("/sales-report").GetProperty("get").GetProperty("responses");
        Assert.True(responses.TryGetProperty("200", out _));
    }
}
