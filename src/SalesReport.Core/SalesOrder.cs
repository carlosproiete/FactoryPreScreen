namespace SalesReport.Core;

public record SalesOrder(
    DateOnly OrderDate,
    DateOnly ShipDate,
    string State,
    Product Product,
    bool Returned,
    decimal Sales,
    int Quantity,
    decimal Discount,
    decimal Profit);
