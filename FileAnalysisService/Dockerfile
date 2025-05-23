FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FileAnalysisService/*.csproj ./FileAnalysisService/
RUN dotnet restore FileAnalysisService/FileAnalysisService.csproj

COPY FileAnalysisService/ ./FileAnalysisService/
WORKDIR /src/FileAnalysisService
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "FileAnalysisService.dll"]
