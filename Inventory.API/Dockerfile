# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Correct project references and folder casing
COPY ["Inventory.API/Inventory.API.csproj", "Inventory.API/"]
COPY ["Inventory.Application/Inventory.Application.csproj", "Inventory.Application/"]
COPY ["Inventory.Core/Inventory.Core.csproj", "Inventory.Core/"]
COPY ["Inventory.Infrastructure/Inventory.Infrastructure.csproj", "Inventory.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Inventory.API/Inventory.API.csproj"

# Copy the full solution source
COPY . .

# Build and publish the project
WORKDIR "/src/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c Release -o /app/build
RUN dotnet publish "Inventory.API.csproj" -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]