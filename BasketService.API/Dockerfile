FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BasketService.API.csproj ./BasketService.API/

RUN dotnet restore ./BasketService.API/BasketService.API.csproj

COPY . ./BasketService.API/

RUN dotnet publish ./BasketService.API/BasketService.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BasketService.API.dll"]
