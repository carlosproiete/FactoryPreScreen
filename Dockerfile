FROM mcr.microsoft.com/dotnet/sdk:10.0.301 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY src/ src/
COPY Input/ Input/
RUN dotnet publish src/SalesReport.Api --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0.15
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "SalesReport.Api.dll"]
