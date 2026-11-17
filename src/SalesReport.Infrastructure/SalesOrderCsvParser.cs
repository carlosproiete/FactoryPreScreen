using System.Globalization;
using CsvHelper;
using SalesReport.Core;

namespace SalesReport.Infrastructure;

public class SalesOrderCsvParser
{
    private const string DateFormat = "M/d/yyyy";

    private static class Column
    {
        public const string OrderDate = "Order Date";
        public const string ShipDate = "Ship Date";
        public const string State = "State";
        public const string ProductId = "Product ID";
        public const string Category = "Category";
        public const string ProductName = "Product Name";
        public const string Returned = "Returned";
        public const string Sales = "Sales";
        public const string Quantity = "Quantity";
        public const string Discount = "Discount";
        public const string Profit = "Profit";
    }

    public IReadOnlyList<SalesOrder> Parse(TextReader reader)
    {
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        if (!csv.Read())
        {
            throw new InvalidSalesDataException(1, "the file is empty");
        }

        csv.ReadHeader();

        var orders = new List<SalesOrder>();

        try
        {
            while (csv.Read())
            {
                orders.Add(ReadOrder(csv));
            }
        }
        catch (CsvHelperException exception)
        {
            throw new InvalidSalesDataException(csv.Parser.RawRow, "the row could not be read as CSV", exception);
        }

        return orders;
    }

    private static SalesOrder ReadOrder(CsvReader csv)
    {
        var orderDate = ReadDate(csv, Column.OrderDate);
        var shipDate = ReadDate(csv, Column.ShipDate);
        var state = ReadText(csv, Column.State);
        var productId = ReadText(csv, Column.ProductId);
        var productName = ReadText(csv, Column.ProductName);
        var category = ReadText(csv, Column.Category);
        var product = new Product(productId, productName, category);
        var returned = ReadReturned(csv, Column.Returned);
        var sales = ReadDecimal(csv, Column.Sales);
        var quantity = ReadInteger(csv, Column.Quantity);
        var discount = ReadDecimal(csv, Column.Discount);
        var profit = ReadDecimal(csv, Column.Profit);

        return new SalesOrder(orderDate, shipDate, state, product, returned, sales, quantity, discount, profit);
    }

    private static string ReadField(CsvReader csv, string column)
    {
        if (csv.HeaderRecord is null || !csv.HeaderRecord.Contains(column))
        {
            throw new InvalidSalesDataException(csv.Parser.RawRow, $"column '{column}' is missing from the header");
        }

        return csv.GetField(column) ?? string.Empty;
    }

    private static string ReadText(CsvReader csv, string column)
    {
        var text = ReadField(csv, column);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw Invalid(csv, column, text, "a value");
        }

        return text;
    }

    private static DateOnly ReadDate(CsvReader csv, string column)
    {
        var text = ReadField(csv, column);

        if (!DateOnly.TryParseExact(text, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            throw Invalid(csv, column, text, $"a date as {DateFormat}");
        }

        return date;
    }

    private static decimal ReadDecimal(CsvReader csv, string column)
    {
        var text = ReadField(csv, column);
        var style = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

        if (!decimal.TryParse(text, style, CultureInfo.InvariantCulture, out var number))
        {
            throw Invalid(csv, column, text, "a number with a dot as the decimal separator");
        }

        return number;
    }

    private static int ReadInteger(CsvReader csv, string column)
    {
        var text = ReadField(csv, column);

        if (!int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number))
        {
            throw Invalid(csv, column, text, "a whole number");
        }

        return number;
    }

    private static bool ReadReturned(CsvReader csv, string column)
    {
        var text = ReadField(csv, column);

        return text.ToLowerInvariant() switch
        {
            "yes" => true,
            "not" => false,
            _ => throw Invalid(csv, column, text, "Yes or Not"),
        };
    }

    private static InvalidSalesDataException Invalid(CsvReader csv, string column, string? text, string expected) =>
        new(csv.Parser.RawRow, $"column '{column}' has '{text}', expected {expected}");
}
