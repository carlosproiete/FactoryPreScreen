using SalesReport.Core;
using SalesReport.Infrastructure;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class IncludedFileReportTests
{
    // Expected values were calculated with a separate script before any of this code was written.
    [Fact]
    public void Report_MatchesTheIndependentCalculation()
    {
        var orders = new FileSalesOrderSource(new SalesOrderCsvParser(), IncludedFilePath).Read();
        var service = new SalesReportService();
        var leastProfitable = new Product("OFF-BI-10004995", "GBC DocuBind P400 Electric Binding System", "Office Supplies");
        var highestDiscount = new Product("OFF-BI-10002609", "Avery Hidden Tab Dividers for Binding Systems", "Office Supplies");
        var mostReturned = new Product("OFF-FA-10000304", "Advantus Push Pins", "Office Supplies");

        var report = service.Calculate(orders);

        Assert.Equal(3024, orders.Count);
        Assert.Equal(leastProfitable, report.LeastProfitableProduct);
        Assert.Equal(highestDiscount, report.ProductWithHighestAverageDiscount);
        Assert.Equal(0.8m, report.HighestAverageDiscount);
        Assert.Equal(mostReturned, report.ProductWithMostReturnedOrders);
        Assert.Equal("Kansas", report.StateWithLowestTotalSales);
        Assert.Equal(726.2126, report.SalesStandardDeviation, precision: 4);
    }
}
