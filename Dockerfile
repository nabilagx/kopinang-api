# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dan restore dependencies
COPY ["kopinang-api.csproj", "./"]
RUN dotnet restore "kopinang-api.csproj"

# Copy semua source code
COPY . .

# Build dan publish
RUN dotnet publish "kopinang-api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "kopinang-api.dll"]
