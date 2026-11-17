namespace SalesReport.Core;

public interface ISalesReportService
{
    SalesReportResult Calculate(IReadOnlyList<SalesOrder> orders);
}
