# ============================================================================
# CityDiscovery Venue Service - Multi-stage Dockerfile
# ============================================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CityDiscovery.VenueService/CityDiscovery.VenueService.csproj", "CityDiscovery.VenueService/"]


RUN dotnet restore "CityDiscovery.VenueService/CityDiscovery.VenueService.csproj"

COPY . .


WORKDIR "/src/CityDiscovery.VenueService"
RUN dotnet build "CityDiscovery.VenueService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CityDiscovery.VenueService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityDiscovery.VenueService.dll"]