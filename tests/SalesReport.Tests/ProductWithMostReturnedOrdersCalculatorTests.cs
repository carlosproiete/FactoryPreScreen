using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class ProductWithMostReturnedOrdersCalculatorTests
{
    private readonly ProductWithMostReturnedOrdersCalculator _calculator = new();

    [Fact]
    public void Calculate_CountsReturnedOrders_NotAllOrders()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, returned: true), Order(ProductA), Order(ProductA), Order(ProductA)]),
            new ProductOrders(ProductB, [Order(ProductB, returned: true), Order(ProductB, returned: true)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }

    [Fact]
    public void Calculate_CountsOrders_NotReturnedUnits()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, returned: true, quantity: 10)]),
            new ProductOrders(ProductB, [Order(ProductB, returned: true, quantity: 1), Order(ProductB, returned: true, quantity: 1)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }

    [Fact]
    public void Calculate_Tie_ReturnsTheFirstProductInTheList()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductB, [Order(ProductB, returned: true)]),
            new ProductOrders(ProductA, [Order(ProductA, returned: true)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result);
    }

    [Fact]
    public void Calculate_ReturnsNull_WhenNothingWasReturned()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA)]),
            new ProductOrders(ProductB, [Order(ProductB)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Null(result);
    }
}
