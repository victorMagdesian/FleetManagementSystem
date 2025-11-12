# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["FleetManager.sln", "./"]

# Copy project files
COPY ["src/FleetManager.Api/FleetManager.Api.csproj", "src/FleetManager.Api/"]
COPY ["src/FleetManager.Application/FleetManager.Application.csproj", "src/FleetManager.Application/"]
COPY ["src/FleetManager.Domain/FleetManager.Domain.csproj", "src/FleetManager.Domain/"]
COPY ["src/FleetManager.Infrastructure/FleetManager.Infrastructure.csproj", "src/FleetManager.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/FleetManager.Api/FleetManager.Api.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/FleetManager.Api"
RUN dotnet build "FleetManager.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FleetManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FleetManager.Api.dll"]
