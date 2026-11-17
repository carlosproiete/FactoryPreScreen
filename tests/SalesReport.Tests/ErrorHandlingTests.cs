using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SalesReport.Core;

namespace SalesReport.Tests;

public class ErrorHandlingTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    public static TheoryData<Exception> Failures => new()
    {
        new InvalidSalesDataException(24, "column 'Sales' has 'abc'"),
        new FileNotFoundException("Could not find file 'C:\\secret\\SalesOrders.csv'."),
        new InvalidOperationException("Cannot build a sales report without orders."),
    };

    [Theory]
    [MemberData(nameof(Failures))]
    public async Task Get_ReturnsAGenericProblem_WithoutInternalDetails(Exception failure)
    {
        var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddSingleton<ISalesOrderSource>(new ThrowingSalesOrderSource(failure))))
            .CreateClient();

        var response = await client.GetAsync("/sales-report");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("The sales report could not be generated.", body);
        Assert.DoesNotContain(failure.Message, body);
        Assert.DoesNotContain("   at ", body);
    }

    private sealed class ThrowingSalesOrderSource(Exception failure) : ISalesOrderSource
    {
        public IReadOnlyList<SalesOrder> Read() => throw failure;
    }
}
