using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class SalesReportEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_ReturnsTheSixFieldsOfTheAssignment_ForTheIncludedFile()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/sales-report");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var properties = json.RootElement.EnumerateObject().Select(property => property.Name);
        string[] expected =
        [
            "leastProfitableProduct",
            "productWithHighestAverageDiscount",
            "highestAverageDiscount",
            "productWithMostReturnedOrders",
            "stateWithLowestTotalSales",
            "salesStandardDeviation",
        ];
        Assert.Equal(expected.Order(), properties.Order());
        Assert.Equal("Kansas", json.RootElement.GetProperty("stateWithLowestTotalSales").GetString());
    }

    [Fact]
    public async Task Get_UsesTheRegisteredSalesOrderSource()
    {
        var source = new FakeSalesOrderSource(
        [
            Order(ProductA, state: "Texas", sales: 10m),
            Order(ProductB, state: "Ohio", sales: 20m),
        ]);
        var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddSingleton<ISalesOrderSource>(source)))
            .CreateClient();

        var report = await client.GetFromJsonAsync<SalesReportResult>("/sales-report");

        Assert.Equal("Texas", report?.StateWithLowestTotalSales);
    }

    private sealed class FakeSalesOrderSource(IReadOnlyList<SalesOrder> orders) : ISalesOrderSource
    {
        public IReadOnlyList<SalesOrder> Read() => orders;
    }
}
