namespace SalesReport.Core;

public class SalesReportService
{
    private readonly LeastProfitableProductCalculator _leastProfitableProductCalculator = new();
    private readonly HighestAverageDiscountCalculator _highestAverageDiscountCalculator = new();
    private readonly ProductWithMostReturnedOrdersCalculator _productWithMostReturnedOrdersCalculator = new();
    private readonly StateWithLowestTotalSalesCalculator _stateWithLowestTotalSalesCalculator = new();
    private readonly SalesStandardDeviationCalculator _salesStandardDeviationCalculator = new();

    public SalesReportResult Calculate(IReadOnlyList<SalesOrder> orders)
    {
        if (orders.Count == 0)
        {
            throw new InvalidOperationException("Cannot build a sales report without orders.");
        }

        var productOrders = orders
            .GroupBy(order => order.Product.Id)
            .Select(group => new ProductOrders(group.First().Product, group.ToList()))
            .ToList();
        var stateOrders = orders.GroupBy(order => order.State).ToList();

        var leastProfitableProduct = _leastProfitableProductCalculator.Calculate(productOrders);
        var highestAverageDiscount = _highestAverageDiscountCalculator.Calculate(productOrders);
        var productWithMostReturnedOrders = _productWithMostReturnedOrdersCalculator.Calculate(productOrders);
        var stateWithLowestTotalSales = _stateWithLowestTotalSalesCalculator.Calculate(stateOrders);
        var salesStandardDeviation = _salesStandardDeviationCalculator.Calculate(orders);

        return new SalesReportResult(
            leastProfitableProduct,
            highestAverageDiscount.Product,
            highestAverageDiscount.AverageDiscount,
            productWithMostReturnedOrders,
            stateWithLowestTotalSales,
            salesStandardDeviation);
    }
}
