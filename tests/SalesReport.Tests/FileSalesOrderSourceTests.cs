using System.Text;
using SalesReport.Core;
using SalesReport.Infrastructure;
using static SalesReport.Tests.TestData;

namespace SalesReport.Tests;

public class FileSalesOrderSourceTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

    public void Dispose() => File.Delete(_path);

    [Fact]
    public void Read_ReadsAFileThatStartsWithAByteOrderMark()
    {
        var headerAndFirstRow = File.ReadLines(IncludedFilePath).Take(2);
        File.WriteAllLines(_path, headerAndFirstRow, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        Assert.Equal([0xEF, 0xBB, 0xBF], File.ReadAllBytes(_path).Take(3));

        var orders = Source().Read();

        Assert.Single(orders);
    }

    [Fact]
    public void Read_Throws_WhenTheFileDoesNotExist()
    {
        Assert.Throws<FileNotFoundException>(() => Source().Read());
    }

    [Fact]
    public void Read_PropagatesAnInvalidFile()
    {
        var header = File.ReadLines(IncludedFilePath).First();
        File.WriteAllLines(_path, [header, "8/11/2016"]);

        var exception = Assert.Throws<InvalidSalesDataException>(() => Source().Read());

        Assert.Equal(2, exception.Line);
    }

    private FileSalesOrderSource Source() => new(_path);
}
