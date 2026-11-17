namespace SalesReport.Core;

public class StateWithLowestTotalSalesCalculator
{
    public string Calculate(IReadOnlyList<IGrouping<string, SalesOrder>> stateOrders)
    {
        var lowest = stateOrders.OrderBy(state => state.Sum(order => order.Sales)).First();

        return lowest.Key;
    }
}
