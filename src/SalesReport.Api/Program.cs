using SalesReport.Api.ErrorHandling;
using SalesReport.Core;
using SalesReport.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuredFilePath = builder.Configuration["SalesData:FilePath"]
    ?? throw new InvalidOperationException("SalesData:FilePath is not configured.");
var salesFilePath = Path.Combine(AppContext.BaseDirectory, configuredFilePath);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<SalesReportExceptionHandler>();
builder.Services.AddSingleton<SalesOrderCsvParser>();
builder.Services.AddSingleton<ISalesOrderSource>(services =>
    new FileSalesOrderSource(services.GetRequiredService<SalesOrderCsvParser>(), salesFilePath));
builder.Services.AddSingleton<ISalesReportService, SalesReportService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/sales-report", (ISalesOrderSource source, ISalesReportService service) =>
{
    var orders = source.Read();
    var report = service.Calculate(orders);

    return report;
});

app.Run();

public partial class Program
{
}
