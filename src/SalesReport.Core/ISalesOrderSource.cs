namespace SalesReport.Core;

public interface ISalesOrderSource
{
    IReadOnlyList<SalesOrder> Read();
}
