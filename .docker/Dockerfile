FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16 AS build
WORKDIR /workspace
COPY ["GoodWeather.sln", "."]

# API projects
COPY ["src/GoodWeather.Api/GoodWeather.Api.csproj", "src/GoodWeather.Api/"]
RUN dotnet restore ./src/GoodWeather.Api/GoodWeather.Api.csproj --configfile ./nuget.config

COPY src src
RUN dotnet build --no-restore -c Release ./src/GoodWeather.Api/GoodWeather.Api.csproj
RUN dotnet publish "src/GoodWeather.Api/GoodWeather.Api.csproj" --no-restore --no-build -c Release -o /app/publish

FROM build AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY .docker/ .
ENTRYPOINT ["dotnet", "GoodWeather.Api.dll"]
