# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY hinos_api.csproj ./
RUN dotnet restore hinos_api.csproj

# Copy source files explicitly to ensure all necessary files are included
# Copy Data folder - copy all .cs files explicitly
COPY Data/DatabaseInitializer.cs ./Data/
COPY Data/DbSeeder.cs ./Data/
COPY Data/HymnsDbContext.cs ./Data/
# Copy Configuration folder
COPY Configuration/ ./Configuration/
# Copy DTOs folder
COPY DTOs/ ./DTOs/
# Copy Endpoints folder
COPY Endpoints/ ./Endpoints/
# Copy Models folder
COPY Models/ ./Models/
# Copy Services folder
COPY Services/ ./Services/
# Copy Properties folder
COPY Properties/ ./Properties/
# Copy Scripts folder
COPY Scripts/ ./Scripts/
# Copy root files
COPY Program.cs ./
COPY appsettings*.json ./

# Build and publish
RUN dotnet publish hinos_api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install wget for healthcheck
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

# Create data directory
RUN mkdir -p /app/data

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "hinos_api.dll"]
