using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class SalesReportServiceTests
{
    private readonly SalesReportService _service = new();

    [Fact]
    public void Calculate_ReturnsTheSixFieldsOfTheReport()
    {
        var productC = new Product("C", "Product C", "Office Supplies");
        var orders = new[]
        {
            Order(ProductA, state: "Texas", sales: 100m, profit: -50m, discount: 0.1m),
            Order(ProductB, state: "Texas", sales: 100m, profit: 10m, discount: 0.5m),
            Order(productC, state: "Ohio", sales: 60m, profit: 5m, discount: 0.2m, returned: true),
            Order(productC, state: "Ohio", sales: 60m, profit: 5m, discount: 0.2m, returned: true),
        };

        var result = _service.Calculate(orders);

        var expected = new SalesReportResult(
            LeastProfitableProduct: ProductA,
            ProductWithHighestAverageDiscount: ProductB,
            HighestAverageDiscount: 0.5m,
            ProductWithMostReturnedOrders: productC,
            StateWithLowestTotalSales: "Ohio",
            SalesStandardDeviation: 20.0);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_GroupsProductsById_WhenTheSameIdHasDifferentNames()
    {
        var renamedProductA = new Product("A", "Other name", "Furniture");
        var orders = new[]
        {
            Order(ProductA, profit: -10m),
            Order(renamedProductA, profit: -10m),
            Order(ProductB, profit: -15m),
        };

        var result = _service.Calculate(orders);

        Assert.Equal(ProductA, result.LeastProfitableProduct);
    }

    [Fact]
    public void Calculate_Tie_FollowsTheOrderOfTheFile()
    {
        var orders = new[]
        {
            Order(ProductB, state: "Texas", sales: 50m, returned: true),
            Order(ProductA, state: "Ohio", sales: 50m, returned: true),
        };

        var result = _service.Calculate(orders);

        Assert.Equal(ProductB, result.LeastProfitableProduct);
        Assert.Equal(ProductB, result.ProductWithHighestAverageDiscount);
        Assert.Equal(ProductB, result.ProductWithMostReturnedOrders);
        Assert.Equal("Texas", result.StateWithLowestTotalSales);
    }

    [Fact]
    public void Calculate_Throws_WhenThereAreNoOrders()
    {
        Assert.Throws<InvalidOperationException>(() => _service.Calculate([]));
    }
}
