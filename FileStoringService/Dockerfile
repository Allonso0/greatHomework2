FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FileStoringService/*.csproj ./FileStoringService/
RUN dotnet restore ./FileStoringService/FileStoringService.csproj

COPY FileStoringService/ ./FileStoringService/
WORKDIR /src/FileStoringService
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "FileStoringService.dll"]
