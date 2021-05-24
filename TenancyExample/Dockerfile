FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR .
COPY . src
RUN dotnet restore "src/TenancyExample.csproj"
RUN dotnet build "src/TenancyExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/TenancyExample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "TenancyExample.dll"]
