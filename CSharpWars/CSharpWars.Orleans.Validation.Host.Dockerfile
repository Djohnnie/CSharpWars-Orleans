FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./CSharpWars.Orleans.Validation.Host/CSharpWars.Orleans.Validation.Host.csproj", "CSharpWars.Orleans.Validation.Host/"]
COPY ["./CSharpWars.Orleans.Validation.Grains/CSharpWars.Orleans.Validation.Grains.csproj", "CSharpWars.Orleans.Validation.Grains/"]
RUN dotnet restore "CSharpWars.Orleans.Validation.Host/CSharpWars.Orleans.Validation.Host.csproj"
COPY . .
WORKDIR "/src/CSharpWars.Orleans.Validation.Host"
RUN dotnet build "CSharpWars.Orleans.Validation.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpWars.Orleans.Validation.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpWars.Orleans.Validation.Host.dll"]