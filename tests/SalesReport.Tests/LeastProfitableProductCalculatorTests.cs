using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class LeastProfitableProductCalculatorTests
{
    private readonly LeastProfitableProductCalculator _calculator = new();

    [Fact]
    public void Calculate_SumsProfitPerProduct_NotPerOrder()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, profit: -30m), Order(ProductA, profit: -30m)]),
            new ProductOrders(ProductB, [Order(ProductB, profit: -50m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductA, result);
    }

    [Fact]
    public void Calculate_UsesTheSignedTotal_NotTheAbsoluteValue()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, profit: 0m)]),
            new ProductOrders(ProductB, [Order(ProductB, profit: -10m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }

    [Fact]
    public void Calculate_IncludesReturnedOrders()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, profit: -10m)]),
            new ProductOrders(ProductB, [Order(ProductB, profit: -5m), Order(ProductB, profit: -20m, returned: true)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }

    [Fact]
    public void Calculate_Tie_ReturnsTheFirstProductInTheList()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductB, [Order(ProductB, profit: -10m)]),
            new ProductOrders(ProductA, [Order(ProductA, profit: -10m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }
}
