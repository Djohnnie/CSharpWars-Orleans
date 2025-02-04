FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./CSharpWars.Orleans.Scaler/CSharpWars.Orleans.Scaler.csproj", "CSharpWars.Orleans.Scaler/"]
RUN dotnet restore "CSharpWars.Orleans.Scaler/CSharpWars.Orleans.Scaler.csproj"
COPY . .
WORKDIR "/src/CSharpWars.Orleans.Scaler"
RUN dotnet build "CSharpWars.Orleans.Scaler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpWars.Orleans.Scaler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpWars.Orleans.Scaler.dll"]