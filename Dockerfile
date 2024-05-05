#ENV ASPNETCORE_URLS=http://+:5000
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16 AS build
WORKDIR /workspace
COPY ["GoodWeather.sln", "."]

# API projects
COPY ["src/GoodWeather.Api/GoodWeather.Api.csproj", "src/GoodWeather.Api/"]
COPY ["src/GoodWeather.Queries/GoodWeather.Queries.csproj", "src/GoodWeather.Queries/"]
COPY ["src/GoodWeather.Common/GoodWeather.Common.csproj", "src/GoodWeather.Common/"]
COPY ["src/GoodWeather.ExternalServices/GoodWeather.ExternalServices.csproj", "src/GoodWeather.ExternalServices/"]
COPY ["src/GoodWeather.Cache/GoodWeather.Cache.csproj", "src/GoodWeather.Cache/"]
RUN dotnet restore ./src/GoodWeather.Api/GoodWeather.Api.csproj

COPY src src
RUN dotnet build --no-restore -c Release ./src/GoodWeather.Api/GoodWeather.Api.csproj
RUN dotnet publish "src/GoodWeather.Api/GoodWeather.Api.csproj" --no-restore --no-build -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "GoodWeather.Api.dll"]
