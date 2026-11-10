# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Preserve the project folder structure when restoring
COPY MyFirstApi/MyFirstApi.csproj MyFirstApi/
RUN dotnet restore MyFirstApi/MyFirstApi.csproj

# Copy the remaining source files
COPY . .

# Publish only the API project
RUN dotnet publish MyFirstApi/MyFirstApi.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "MyFirstApi.dll"]