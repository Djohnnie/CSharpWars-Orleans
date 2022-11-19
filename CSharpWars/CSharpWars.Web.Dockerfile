FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CSharpWars.Web/CSharpWars.Web.csproj", "CSharpWars.Web/"]
COPY ["CSharpWars.Orleans.Grains/CSharpWars.Orleans.Grains.csproj", "CSharpWars.Orleans.Grains/"]
RUN dotnet restore "CSharpWars.Web/CSharpWars.Web.csproj"
COPY . .
WORKDIR "/src/CSharpWars.Web"
RUN dotnet build "CSharpWars.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpWars.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpWars.Web.dll"]