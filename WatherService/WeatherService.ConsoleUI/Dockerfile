FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WeatherService.Console/WeatherService.Console.csproj", "WeatherService.Console/"]
RUN dotnet restore "WeatherService.Console/WeatherService.Console.csproj"
COPY . .
WORKDIR "/src/WeatherService.Console"
RUN dotnet build "WeatherService.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherService.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherService.Console.dll"]
