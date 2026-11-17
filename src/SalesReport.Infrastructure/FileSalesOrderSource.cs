using SalesReport.Core;

namespace SalesReport.Infrastructure;

public class FileSalesOrderSource(SalesOrderCsvParser parser, string filePath) : ISalesOrderSource
{
    public IReadOnlyList<SalesOrder> Read()
    {
        using var reader = new StreamReader(filePath);

        return parser.Parse(reader);
    }
}
