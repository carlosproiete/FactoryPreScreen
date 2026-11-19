using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class UploadEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Header = "Order Date,Ship Date,State,Product ID,Category,Product Name,Returned,Sales,Quantity,Discount,Profit";

    [Fact]
    public async Task Post_WithTheIncludedFile_ReturnsTheSixValues()
    {
        var client = factory.CreateClient();
        var file = new ByteArrayContent(await File.ReadAllBytesAsync(IncludedFilePath));

        var response = await client.PostAsync("/sales-report", Upload(file));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<SalesReportResult>();
        Assert.NotNull(report);
        Assert.Equal("OFF-BI-10004995", report.LeastProfitableProduct.Id);
        Assert.Equal("OFF-BI-10002609", report.ProductWithHighestAverageDiscount.Id);
        Assert.Equal(0.8m, report.HighestAverageDiscount);
        Assert.Equal("OFF-FA-10000304", report.ProductWithMostReturnedOrders?.Id);
        Assert.Equal("Kansas", report.StateWithLowestTotalSales);
        Assert.Equal(726.2126, report.SalesStandardDeviation, 4);
    }

    [Fact]
    public async Task Post_WithADifferentFile_ReturnsItsOwnReport()
    {
        var client = factory.CreateClient();
        var csv = string.Join("\n",
            Header,
            "1/5/2020,1/7/2020,Texas,P-1,Office,Pen,Not,10.00,1,0.00,2.00",
            "1/6/2020,1/8/2020,Ohio,P-2,Office,Pad,Not,20.00,1,0.00,3.00");

        var response = await client.PostAsync("/sales-report", Upload(new StringContent(csv)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<SalesReportResult>();
        Assert.NotNull(report);
        Assert.Equal("Texas", report.StateWithLowestTotalSales);
        Assert.Equal("P-1", report.LeastProfitableProduct.Id);
    }

    [Fact]
    public async Task Post_WithAnInvalidRow_Returns400WithLineAndColumn()
    {
        var client = factory.CreateClient();
        var csv = string.Join("\n",
            Header,
            "1/5/2020,1/7/2020,Texas,P-1,Office,Pen,Not,10.00,1,0.00,2.00",
            "1/6/2020,1/8/2020,Ohio,P-2,Office,Pad,Not,abc,1,0.00,3.00");

        var response = await client.PostAsync("/sales-report", Upload(new StringContent(csv)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var detail = json.RootElement.GetProperty("detail").GetString();
        Assert.Contains("line 3", detail);
        Assert.Contains("'Sales'", detail);
    }

    [Fact]
    public async Task Post_WithoutAFile_Returns400()
    {
        var client = factory.CreateClient();
        var form = new MultipartFormDataContent { { new StringContent("x"), "other" } };

        var response = await client.PostAsync("/sales-report", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithAHeaderOnlyFile_Returns400()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsync("/sales-report", Upload(new StringContent(Header)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Contains("no orders", json.RootElement.GetProperty("detail").GetString());
    }

    private static MultipartFormDataContent Upload(HttpContent file)
    {
        var form = new MultipartFormDataContent();
        form.Add(file, "file", "sales.csv");

        return form;
    }
}
