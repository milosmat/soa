# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./PurchaseApi/PurchaseApi.csproj ./PurchaseApi/
COPY ./Contracts/Contracts.csproj ./Contracts/
RUN dotnet restore ./PurchaseApi/PurchaseApi.csproj

COPY ./PurchaseApi ./PurchaseApi
COPY ./Contracts ./Contracts

RUN dotnet publish ./PurchaseApi/PurchaseApi.csproj -c Release -o /app/publish

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","PurchaseApi.dll"]
