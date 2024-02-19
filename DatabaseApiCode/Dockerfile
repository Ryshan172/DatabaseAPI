# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official ASP.NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy files and restore
COPY . .
RUN dotnet restore

# Build the application
WORKDIR /app
RUN dotnet build -c Release

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# Start the application
ENTRYPOINT ["dotnet", "./bin/Debug/net8.0/DatabaseApiCode.dll"]
