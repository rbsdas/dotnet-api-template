FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo

COPY ApiTemplate.sln Directory.Build.props Directory.Packages.props ./
COPY src/ApiTemplate.Api/ApiTemplate.Api.csproj src/ApiTemplate.Api/
COPY src/ApiTemplate.Application/ApiTemplate.Application.csproj src/ApiTemplate.Application/
COPY src/ApiTemplate.Infrastructure/ApiTemplate.Infrastructure.csproj src/ApiTemplate.Infrastructure/
COPY src/ApiTemplate.Domain/ApiTemplate.Domain.csproj src/ApiTemplate.Domain/
RUN dotnet restore src/ApiTemplate.Api/ApiTemplate.Api.csproj

COPY src/ src/
RUN dotnet publish src/ApiTemplate.Api/ApiTemplate.Api.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ApiTemplate.Api.dll"]
