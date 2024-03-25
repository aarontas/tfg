FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /workspace
COPY ["GoodWeather.sln", "."]
COPY ["nuget.config", "."]
COPY ["Directory.Build.props", "."]

# API projects
COPY ["src/GoodWeather.Api/GoodWeather.Api.csproj", "src/GoodWeather.Api/"]
RUN dotnet restore ./src/GoodWeather.Api/GoodWeather.Api.csproj

COPY src src
RUN dotnet build --no-restore -c Release ./src/GoodWeather.Api/GoodWeather.Api.csproj
RUN dotnet publish "src/GoodWeather.Api/GoodWeather.Api.csproj" --no-restore --no-build -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoodWeather.Api.dll"]
