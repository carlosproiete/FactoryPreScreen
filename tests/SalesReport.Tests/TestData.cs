using SalesReport.Core;

namespace SalesReport.Tests;

internal static class TestData
{
    public static readonly Product ProductA = new("A", "Product A", "Furniture");
    public static readonly Product ProductB = new("B", "Product B", "Technology");

    public static readonly string IncludedFilePath = Path.Combine(AppContext.BaseDirectory, "Input", "SalesOrders.csv");

    public static SalesOrder Order(
        Product product,
        DateOnly orderDate = default,
        DateOnly shipDate = default,
        string state = "",
        bool returned = false,
        decimal sales = 0m,
        int quantity = 1,
        decimal discount = 0m,
        decimal profit = 0m) =>
        new(orderDate, shipDate, state, product, returned, sales, quantity, discount, profit);
}
