# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Install necessary build dependencies
RUN apk add --no-cache icu-libs

# Copy csproj and restore dependencies
COPY ["Abjjad.Images.API/Abjjad.Images.API.csproj", "Abjjad.Images.API/"]
COPY ["Abjjad.Images/Abjjad.Images.csproj", "Abjjad.Images/"]
RUN dotnet restore "Abjjad.Images.API/Abjjad.Images.API.csproj" --runtime linux-musl-x64

# Copy source code and build
COPY ["Abjjad.Images.API/", "Abjjad.Images.API/"]
COPY ["Abjjad.Images/", "Abjjad.Images/"]
COPY ["Abjjad.Images.Core/", "Abjjad.Images.Core/"]
RUN dotnet build "Abjjad.Images.API/Abjjad.Images.API.csproj" -c Release -o /app/build --runtime linux-musl-x64

# Publish stage
FROM build AS publish
RUN dotnet publish "Abjjad.Images.API/Abjjad.Images.API.csproj" -c Release -o /app/publish --runtime linux-musl-x64 /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Install necessary runtime dependencies
RUN apk add --no-cache icu-libs

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Abjjad.Images.API.dll"] 