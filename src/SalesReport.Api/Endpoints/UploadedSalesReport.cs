using SalesReport.Core;
using SalesReport.Infrastructure;

namespace SalesReport.Api.Endpoints;

public class UploadedSalesReport(SalesReportService service)
{
    private readonly SalesOrderCsvParser _parser = new();

    public IResult Create(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var orders = _parser.Parse(reader);

            if (orders.Count == 0)
            {
                return BadRequest("The file has no orders.");
            }

            var report = service.Calculate(orders);

            return Results.Ok(report);
        }
        catch (InvalidSalesDataException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static IResult BadRequest(string detail)
    {
        return Results.Problem(
            title: "The uploaded file cannot be used.",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest);
    }
}
