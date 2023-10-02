FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./CSharpWars.Orleans.Host/CSharpWars.Orleans.Host.csproj", "CSharpWars.Orleans.Host/"]
COPY ["./CSharpWars.Orleans.Grains/CSharpWars.Orleans.Grains.csproj", "CSharpWars.Orleans.Grains/"]
RUN dotnet restore "CSharpWars.Orleans.Host/CSharpWars.Orleans.Host.csproj"
COPY . .
WORKDIR "/src/CSharpWars.Orleans.Host"
RUN dotnet build "CSharpWars.Orleans.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpWars.Orleans.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpWars.Orleans.Host.dll"]