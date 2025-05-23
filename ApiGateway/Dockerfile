FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ApiGateway/*.csproj ./ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

COPY ApiGateway/ ./ApiGateway/
WORKDIR /src/ApiGateway
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
