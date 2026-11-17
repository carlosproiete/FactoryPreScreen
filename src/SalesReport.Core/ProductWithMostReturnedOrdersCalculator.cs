namespace SalesReport.Core;

public class ProductWithMostReturnedOrdersCalculator
{
    public Product? Calculate(IReadOnlyList<ProductOrders> productOrders)
    {
        var mostReturned = productOrders.OrderByDescending(product => product.Orders.Count(order => order.Returned)).First();
        var hasReturns = mostReturned.Orders.Any(order => order.Returned);

        if (!hasReturns)
        {
            return null;
        }

        return mostReturned.Product;
    }
}
