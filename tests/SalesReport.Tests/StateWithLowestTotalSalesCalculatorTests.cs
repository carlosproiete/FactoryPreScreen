using SalesReport.Core;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class StateWithLowestTotalSalesCalculatorTests
{
    private readonly StateWithLowestTotalSalesCalculator _calculator = new();

    [Fact]
    public void Calculate_SumsSalesPerState_NotPerOrder()
    {
        var stateOrders = new[]
        {
            Order(ProductA, state: "Texas", sales: 60m),
            Order(ProductA, state: "Ohio", sales: 100m),
            Order(ProductA, state: "Texas", sales: 60m),
        }.GroupBy(order => order.State).ToList();

        var result = _calculator.Calculate(stateOrders);

        Assert.Equal("Ohio", result);
    }

    [Fact]
    public void Calculate_UsesSalesAsTheLineTotal_NotMultipliedByQuantity()
    {
        var stateOrders = new[]
        {
            Order(ProductA, state: "Texas", sales: 100m, quantity: 1),
            Order(ProductA, state: "Ohio", sales: 60m, quantity: 5),
        }.GroupBy(order => order.State).ToList();

        var result = _calculator.Calculate(stateOrders);

        Assert.Equal("Ohio", result);
    }

    [Fact]
    public void Calculate_IncludesReturnedOrders()
    {
        var stateOrders = new[]
        {
            Order(ProductA, state: "Texas", sales: 100m),
            Order(ProductA, state: "Ohio", sales: 60m),
            Order(ProductA, state: "Ohio", sales: 50m, returned: true),
        }.GroupBy(order => order.State).ToList();

        var result = _calculator.Calculate(stateOrders);

        Assert.Equal("Texas", result);
    }

    [Fact]
    public void Calculate_Tie_ReturnsTheFirstStateInTheList()
    {
        var stateOrders = new[]
        {
            Order(ProductA, state: "Texas", sales: 50m),
            Order(ProductA, state: "Ohio", sales: 50m),
        }.GroupBy(order => order.State).ToList();

        var result = _calculator.Calculate(stateOrders);

        Assert.Equal("Texas", result);
    }
}
