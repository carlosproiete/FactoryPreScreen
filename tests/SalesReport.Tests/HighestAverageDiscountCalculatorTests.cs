using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class HighestAverageDiscountCalculatorTests
{
    private readonly HighestAverageDiscountCalculator _calculator = new();

    [Fact]
    public void Calculate_UsesTheMeanOfTheProductOrders_NotTheHighestSingleDiscount()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, discount: 0.2m), Order(ProductA, discount: 0.6m)]),
            new ProductOrders(ProductB, [Order(ProductB, discount: 0.5m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result.Product);
    }

    [Fact]
    public void Calculate_ReturnsTheAverageOfTheWinningProduct()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, discount: 0.6m), Order(ProductA, discount: 0.8m)]),
            new ProductOrders(ProductB, [Order(ProductB, discount: 0.5m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductA, result.Product);
        Assert.Equal(0.7m, result.AverageDiscount);
    }

    [Fact]
    public void Calculate_ExactTie_ReturnsTheFirstProductInTheList()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, discount: 0.8m)]),
            new ProductOrders(ProductB, [Order(ProductB, discount: 0.8m), Order(ProductB, discount: 0.8m), Order(ProductB, discount: 0.8m)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductA, result.Product);
        Assert.Equal(0.8m, result.AverageDiscount);
    }

    [Fact]
    public void Calculate_IncludesReturnedOrders()
    {
        var productOrders = new[]
        {
            new ProductOrders(ProductA, [Order(ProductA, discount: 0.5m)]),
            new ProductOrders(ProductB, [Order(ProductB, discount: 0.2m), Order(ProductB, discount: 0.9m, returned: true)]),
        };

        var result = _calculator.Calculate(productOrders);

        Assert.Equal(ProductB, result.Product);
    }
}
