# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./ToursApi/ToursApi.csproj ./ToursApi/
COPY ./Contracts/Contracts.csproj ./Contracts/
RUN dotnet restore ./ToursApi/ToursApi.csproj

COPY ./ToursApi ./ToursApi
COPY ./Contracts ./Contracts

RUN dotnet publish ./ToursApi/ToursApi.csproj -c Release -o /app/publish

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","ToursApi.dll"]
