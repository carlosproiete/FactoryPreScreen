using SalesReport.Api.Endpoints;
using SalesReport.Api.ErrorHandling;
using SalesReport.Core;
using SalesReport.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuredFilePath = builder.Configuration["SalesData:FilePath"]
    ?? throw new InvalidOperationException("SalesData:FilePath is not configured.");
var salesFilePath = Path.Combine(AppContext.BaseDirectory, configuredFilePath);

builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<SalesReportExceptionHandler>();
builder.Services.AddSingleton<ISalesOrderSource>(new FileSalesOrderSource(salesFilePath));
builder.Services.AddSingleton<SalesReportService>();
builder.Services.AddSingleton<UploadedSalesReport>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Sales Report API"));
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapGet("/sales-report", (ISalesOrderSource source, SalesReportService service) =>
{
    var orders = source.Read();
    var report = service.Calculate(orders);

    return report;
});

app.MapPost("/sales-report", (IFormFile file, UploadedSalesReport upload) => upload.Create(file)).DisableAntiforgery();

app.Run();

public partial class Program
{
}
