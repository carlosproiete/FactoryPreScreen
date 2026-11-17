namespace SalesReport.Core;

public record SalesReportResult(
    Product LeastProfitableProduct,
    Product ProductWithHighestAverageDiscount,
    decimal HighestAverageDiscount,
    Product? ProductWithMostReturnedOrders,
    string StateWithLowestTotalSales,
    double SalesStandardDeviation);
