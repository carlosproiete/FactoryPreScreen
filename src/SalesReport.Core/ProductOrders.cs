namespace SalesReport.Core;

public record ProductOrders(Product Product, IReadOnlyList<SalesOrder> Orders);
