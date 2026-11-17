namespace SalesReport.Core;

public class HighestAverageDiscountCalculator
{
    public ProductAverageDiscount Calculate(IReadOnlyList<ProductOrders> productOrders)
    {
        var averageDiscounts = productOrders.Select(product => new ProductAverageDiscount(product.Product, product.Orders.Average(order => order.Discount)));
        var highest = averageDiscounts.OrderByDescending(discount => discount.AverageDiscount).First();

        return highest;
    }
}
