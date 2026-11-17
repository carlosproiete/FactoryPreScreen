namespace SalesReport.Core;

public class SalesStandardDeviationCalculator
{
    public double Calculate(IReadOnlyList<SalesOrder> orders)
    {
        var mean = orders.Average(order => order.Sales);
        var variance = orders.Sum(order => (order.Sales - mean) * (order.Sales - mean)) / orders.Count;
        var standardDeviation = Math.Sqrt((double)variance);

        return standardDeviation;
    }
}
