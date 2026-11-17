using SalesReport.Core;

namespace SalesReport.Infrastructure;

public class FileSalesOrderSource(string filePath) : ISalesOrderSource
{
    private readonly SalesOrderCsvParser _parser = new();

    public IReadOnlyList<SalesOrder> Read()
    {
        using var reader = new StreamReader(filePath);

        return _parser.Parse(reader);
    }
}
