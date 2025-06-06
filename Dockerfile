# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["kopinang-api.csproj", "./"]
RUN dotnet restore "kopinang-api.csproj"
COPY . .
RUN dotnet publish "kopinang-api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "kopinang-api.dll"]
