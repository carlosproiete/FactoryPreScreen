namespace SalesReport.Core;

public class InvalidSalesDataException(int line, string reason, Exception? innerException = null)
    : Exception($"Invalid sales data at line {line}: {reason}", innerException)
{
    public int Line { get; } = line;
}
