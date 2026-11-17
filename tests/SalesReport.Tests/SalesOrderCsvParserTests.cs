using System.Globalization;
using SalesReport.Core;
using SalesReport.Infrastructure;

namespace SalesReport.Tests;

public class SalesOrderCsvParserTests
{
    private const string Header = "Order Date,Ship Date,State,Product ID,Category,Product Name,Returned,Sales,Quantity,Discount,Profit,,";

    private static readonly string[] Columns =
    [
        "Order Date", "Ship Date", "State", "Product ID", "Category", "Product Name",
        "Returned", "Sales", "Quantity", "Discount", "Profit",
    ];

    private static readonly string[] ValidValues =
    [
        "8/11/2016", "11/11/2016", "Kentucky", "FUR-BO-10001798", "Furniture", "Bush Somerset Collection Bookcase",
        "Not", "261.96", "2", "0.00", "41.91",
    ];

    private readonly SalesOrderCsvParser _parser = new();

    [Fact]
    public void Parse_ReadsEveryColumnOfARow()
    {
        var orders = _parser.Parse(Csv(RowWith("State", "Kentucky")));

        var expected = new SalesOrder(
            OrderDate: new DateOnly(2016, 8, 11),
            ShipDate: new DateOnly(2016, 11, 11),
            State: "Kentucky",
            Product: new Product("FUR-BO-10001798", "Bush Somerset Collection Bookcase", "Furniture"),
            Returned: false,
            Sales: 261.96m,
            Quantity: 2,
            Discount: 0.00m,
            Profit: 41.91m);
        Assert.Equal(expected, Assert.Single(orders));
    }

    [Fact]
    public void Parse_KeepsTheOrderOfTheFile()
    {
        var orders = _parser.Parse(Csv(RowWith("State", "Texas"), RowWith("State", "Ohio"), RowWith("State", "Utah")));

        Assert.Equal(["Texas", "Ohio", "Utah"], orders.Select(order => order.State));
    }

    [Fact]
    public void Parse_HandlesCommasAndEscapedQuotesInTheProductName()
    {
        var fieldAsWrittenInTheFile = "\"\"\"While you Were Out\"\" Message Book, One Form per Page\"";
        var orders = _parser.Parse(Csv(RowWith("Product Name", fieldAsWrittenInTheFile)));

        Assert.Equal("\"While you Were Out\" Message Book, One Form per Page", Assert.Single(orders).Product.Name);
    }

    [Theory]
    [InlineData("Yes", true)]
    [InlineData("yes", true)]
    [InlineData("Not", false)]
    [InlineData("not", false)]
    public void Parse_AcceptsAnyCaseOfReturned(string value, bool expected)
    {
        var orders = _parser.Parse(Csv(RowWith("Returned", value)));

        Assert.Equal(expected, Assert.Single(orders).Returned);
    }

    [Fact]
    public void Parse_IgnoresTheTrailingUnnamedColumnsAndAStrayValue()
    {
        var orders = _parser.Parse(Csv(RowWith("State", "Nebraska") + "not"));

        Assert.Equal("Nebraska", Assert.Single(orders).State);
    }

    [Fact]
    public void Parse_AcceptsANegativeProfit()
    {
        var orders = _parser.Parse(Csv(RowWith("Profit", "-383.03")));

        Assert.Equal(-383.03m, Assert.Single(orders).Profit);
    }

    [Fact]
    public void Parse_ReadsDatesMonthFirst_RegardlessOfTheCurrentCulture()
    {
        WithCulture("pt-BR", () =>
        {
            var orders = _parser.Parse(Csv(RowWith("Order Date", "9/12/2016")));

            Assert.Equal(new DateOnly(2016, 9, 12), Assert.Single(orders).OrderDate);
        });
    }

    [Fact]
    public void Parse_ReadsNumbersWithADotDecimalSeparator_RegardlessOfTheCurrentCulture()
    {
        WithCulture("pt-BR", () =>
        {
            var orders = _parser.Parse(Csv(RowWith("Sales", "1706.18")));

            Assert.Equal(1706.18m, Assert.Single(orders).Sales);
        });
    }

    [Theory]
    [InlineData("Order Date", "2016-08-11")]
    [InlineData("Ship Date", "")]
    [InlineData("State", "")]
    [InlineData("Product ID", "")]
    [InlineData("Category", "")]
    [InlineData("Product Name", "")]
    [InlineData("Returned", "Maybe")]
    [InlineData("Sales", "abc")]
    [InlineData("Quantity", "1.5")]
    [InlineData("Discount", "")]
    [InlineData("Profit", "n/a")]
    public void Parse_RejectsAnUnreadableField_NamingTheLineAndTheColumn(string column, string value)
    {
        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(Csv(RowWith(column, value))));

        Assert.Equal(2, exception.Line);
        Assert.Contains($"'{column}'", exception.Message);
    }

    [Fact]
    public void Parse_ReportsTheLineOfTheBadRow()
    {
        var csv = Csv(RowWith("State", "Texas"), RowWith("State", "Ohio"), RowWith("Sales", "abc"));

        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(csv));

        Assert.Equal(4, exception.Line);
    }

    [Fact]
    public void Parse_RejectsAFileWithAMissingColumn()
    {
        var csv = new StringReader("Order Date,Ship Date,State,Product ID,Category,Product Name,Returned,Sales,Quantity,Discount,,\n" + RowWith("State", "Kentucky"));

        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(csv));

        Assert.Equal(2, exception.Line);
        Assert.Contains("'Profit'", exception.Message);
    }

    [Fact]
    public void Parse_RejectsARowWithFewerFieldsThanTheHeader()
    {
        var csv = Csv("8/11/2016,11/11/2016,Kentucky");

        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(csv));

        Assert.Equal(2, exception.Line);
        Assert.IsAssignableFrom<CsvHelper.CsvHelperException>(exception.InnerException);
    }

    [Fact]
    public void Parse_RejectsAMalformedQuote_NamingTheLine()
    {
        var csv = Csv(RowWith("Product Name", "Bush \"Somerset Collection"));

        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(csv));

        Assert.Equal(2, exception.Line);
        Assert.IsAssignableFrom<CsvHelper.CsvHelperException>(exception.InnerException);
    }

    [Fact]
    public void Parse_RejectsAnEmptyFile()
    {
        var exception = Assert.Throws<InvalidSalesDataException>(() => _parser.Parse(new StringReader(string.Empty)));

        Assert.Equal(1, exception.Line);
    }

    private static StringReader Csv(params string[] rows) => new(string.Join("\n", [Header, .. rows]));

    private static string RowWith(string column, string value)
    {
        var values = ValidValues.ToArray();
        values[Array.IndexOf(Columns, column)] = value;

        return string.Join(",", values) + ",,";
    }

    private static void WithCulture(string name, Action action)
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);

        try
        {
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
