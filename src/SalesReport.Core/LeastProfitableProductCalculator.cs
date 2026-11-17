namespace SalesReport.Core;

public class LeastProfitableProductCalculator
{
    public Product Calculate(IReadOnlyList<ProductOrders> productOrders)
    {
        var leastProfitable = productOrders.OrderBy(product => product.Orders.Sum(order => order.Profit)).First();

        return leastProfitable.Product;
    }
}
