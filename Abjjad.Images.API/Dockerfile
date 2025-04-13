FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["Abjjad.Images.API/Abjjad.Images.API.csproj", "Abjjad.Images.API/"]
COPY ["Abjjad.Images/Abjjad.Images.csproj", "Abjjad.Images/"]
COPY ["Abjjad.Images.Core/Abjjad.Images.Core.csproj", "Abjjad.Images.Core/"]
RUN dotnet restore "Abjjad.Images.API/Abjjad.Images.API.csproj" --runtime linux-musl-x64

COPY . .
RUN dotnet publish "Abjjad.Images.API/Abjjad.Images.API.csproj" -c Release -o /app/publish --runtime linux-musl-x64 /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Abjjad.Images.API.dll"]