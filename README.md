# Sales Report API

[![CI](https://github.com/carlosproiete/FactoryPreScreen/actions/workflows/ci.yml/badge.svg)](https://github.com/carlosproiete/FactoryPreScreen/actions/workflows/ci.yml)

[Jump to the original instructions](#original-instructions)

## How to run

Requires the .NET 10 SDK.

```
dotnet run --project src/SalesReport.Api
```

Open http://localhost:5065. The root redirects to Swagger UI. The report is at `GET /sales-report`.

`dotnet watch run --project src/SalesReport.Api` does the same and also opens the browser. Swagger UI exists only in the Development environment.

To run the tests:

```
dotnet test
```

## Expected result for the included file

| Field | Value |
|---|---|
| Least profitable product | `OFF-BI-10004995` GBC DocuBind P400 Electric Binding System |
| Product with the highest average discount | `OFF-BI-10002609` Avery Hidden Tab Dividers for Binding Systems |
| Highest average discount | 0.8 |
| Product with the most returned orders | `OFF-FA-10000304` Advantus Push Pins |
| State with the lowest total sales | Kansas |
| Sales standard deviation | 726.2126 |

## Definition of Done

* Solution builds with no warnings
* All tests pass (`dotnet test`)
* The 6 values match the table above
* Every finding in the input file has a test
* Runs with one command, on any OS and machine culture
* Errors do not leak stack traces

## Not requested, nice to have

* Swagger UI
* GitHub Actions workflow running `dotnet build` and `dotnet test` on every push and pull request
* Smoke test step that starts the API and calls the endpoint
* Build badge at the top of this README

All four are done.

## Findings in the input file

* Product names contain commas (649 rows) and escaped quotes (238 rows). Splitting on commas breaks them.
* `Returned` has four spellings: `Not`, `not`, `Yes`, `yes`. An exact match on `Yes` misses more than half of the returns.
* The header ends with two unnamed columns, and line 24 has a stray value in the last one.
* The file starts with a UTF-8 byte order mark.
* Numbers use a dot as decimal separator. Parsing must not depend on the machine culture.
* Dates are `M/d/yyyy`. 1,202 of them are ambiguous (day and month both 12 or less), so they must be read with an explicit format.
* `Profit` is negative on 533 of the 3,024 rows. That is valid data, not an error.
* 19 Product IDs have more than one name and 13 names have more than one ID.
* The assignment writes the path as `Input\SalesOrders.csv`. A backslash breaks on Linux and macOS.
* The `.gitignore` and `.gitattributes` listed as included were not in the package.

## Assumptions

* Least profitable is the lowest signed sum of `Profit` per product. "Absolute" is read as an amount in currency, not as a margin and not as `|x|`.
* Products are grouped by Product ID. The name comes from the first row of that product.
* A tie goes to the product that appears first in the file. Ties are real here: 16 products share the highest average discount and 4 share the highest return count.
* "Returned orders" counts orders, not units.
* All orders count for every field, except the returned orders count.
* Standard deviation is the population one, over all 3,024 records. The sample one would be 726.3327.
* A row that cannot be interpreted rejects the whole file, naming the line. The response has no place to say that rows were skipped.
* When the report cannot be produced, the response is a generic 500. The detail goes to the log, never to the client.

## Plan

1. Scaffold the solution: API project and test project.
2. Calculations and tests.
3. CSV reading with tests.
4. API endpoint, error handling, Swagger.
5. CI.
6. Update this README.

## Dependencies

```
SalesReport.Api             -> Core, Infrastructure
SalesReport.Infrastructure  -> Core
SalesReport.Core            -> nothing
SalesReport.Tests           -> Api, Core, Infrastructure
```

Core has no packages. It holds the model, the calculators and the `ISalesOrderSource` port. Infrastructure reads the CSV (CsvHelper 33.1.0). Api is the composition root (minimal API, `Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore.SwaggerUI`). Tests use xUnit and `Microsoft.AspNetCore.Mvc.Testing`.

Dependency injection follows one rule: inject what is volatile (reading the file), build what is stable (the calculators). That is why the calculators are plain classes and reading the file sits behind the `ISalesOrderSource` port.

## What I'd do differently in production

* Validate value ranges when reading the file: discount between 0 and 1, positive sales and quantity, ship date not before the order date. Today only the format of each field is checked. The assignment does not ask for it and the included file has no violations (checked on all 3,024 rows).
* Stream the file instead of reading all rows into memory, if files were much larger. 3,024 rows fit easily.
* Make the read asynchronous and accept a `CancellationToken`.
* Cache the report until the file changes.

## Original instructions

## Impinj Coding Assesment:

```
TIME:            2-3 hours
LANGUAGES:       C# preferred
FRAMEWORKS:      NET or NET CORE preferred,
TEST FRAMEWORKS: MS BUILD/NUNIT/XUNIT preferred
TYPE:            Web Api
FILES:           In repository
TESTS:           preferred
SUBMIT:          ZipFile, Github Repository Link
```

## Overview:
This exercise is to asses a candidates ability to design and implement a solution to a problem in a given period of time.

We want to see a demonstration of your thought process, skills and development experiences.

This solution should be able to run locally.

_Be prepared to explain your design decisions and architecture!_


## Problem:
Create a web API that can parse a sales record file into an object. The API should return an object with the following fields:
* The least profitable product
* The product with the highest average discount
* The value of the highest average discount
* The product with the highest number of returned orders
* The state with the lowest total sales value
* The sales standard deviation

## Observations:
* Consider the least profitable product the one with the lowest total/absolute profit.
* If there is a tie, return the first product in the processed list.
* Include all orders, regardless of returns/devolutions, for all fields, except when calculating returns.
* The sales standard deviation is calculated using the Sales column (total sale value per record).

## Included:
* Sample input at `Input\SalesOrders.csv`
* A git ignore file for a C# project
* A git attributes file

<br></br>

_Hint: a web api skeleton can be created with `dotnet new webapi`_
