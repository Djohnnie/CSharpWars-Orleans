FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["CSharpWars.WebApi/CSharpWars.WebApi.csproj", "CSharpWars.WebApi/"]
COPY ["CSharpWars.Orleans.Grains/CSharpWars.Orleans.Grains.csproj", "CSharpWars.Orleans.Grains/"]
RUN dotnet restore "CSharpWars.WebApi/CSharpWars.WebApi.csproj"
COPY . .
WORKDIR "/src/CSharpWars.WebApi"
RUN dotnet build "CSharpWars.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpWars.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpWars.WebApi.dll"]