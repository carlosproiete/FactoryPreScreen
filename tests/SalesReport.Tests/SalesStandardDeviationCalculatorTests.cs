using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class SalesStandardDeviationCalculatorTests
{
    private readonly SalesStandardDeviationCalculator _calculator = new();

    [Fact]
    public void Calculate_IsThePopulationStandardDeviation()
    {
        var orders = new[] { 2m, 4m, 4m, 4m, 5m, 5m, 7m, 9m }
            .Select(sales => Order(ProductA, sales: sales))
            .ToList();

        var result = _calculator.Calculate(orders);

        Assert.Equal(2.0, result, precision: 10);
    }

    [Fact]
    public void Calculate_OfASingleOrder_IsZero()
    {
        var orders = new[] { Order(ProductA, sales: 42m) };

        var result = _calculator.Calculate(orders);

        Assert.Equal(0.0, result, precision: 10);
    }

    [Fact]
    public void Calculate_IncludesReturnedOrders()
    {
        var orders = new[]
        {
            Order(ProductA, sales: 2m, returned: true),
            Order(ProductA, sales: 8m),
        };

        var result = _calculator.Calculate(orders);

        Assert.Equal(3.0, result, precision: 10);
    }

    [Fact]
    public void Calculate_UsesEachOrderSales_NotTotalsPerProduct()
    {
        var orders = new[]
        {
            Order(ProductA, sales: 2m),
            Order(ProductA, sales: 4m),
        };

        var result = _calculator.Calculate(orders);

        Assert.Equal(1.0, result, precision: 10);
    }
}
