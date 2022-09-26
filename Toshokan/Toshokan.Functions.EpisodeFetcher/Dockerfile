#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Toshokan.Functions.EpisodeFetcher/Toshokan.Functions.EpisodeFetcher.csproj", "Toshokan.Functions.EpisodeFetcher/"]
COPY ["Toshokan.Libraries.Data/Toshokan.Libraries.Data.csproj", "Toshokan.Libraries.Data/"]
COPY ["Toshokan.Libraries.Models/Toshokan.Libraries.Models.csproj", "Toshokan.Libraries.Models/"]
COPY ["Toshokan.Libraries.Services/Toshokan.Libraries.Services.csproj", "Toshokan.Libraries.Services/"]
COPY ["Toshokan.Libraries.Shared/Toshokan.Libraries.Shared.csproj", "Toshokan.Libraries.Shared/"]
RUN dotnet restore "Toshokan.Functions.EpisodeFetcher/Toshokan.Functions.EpisodeFetcher.csproj"
COPY . .
WORKDIR "/src/Toshokan.Functions.EpisodeFetcher"
RUN dotnet build "Toshokan.Functions.EpisodeFetcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Toshokan.Functions.EpisodeFetcher.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Toshokan.Functions.EpisodeFetcher.dll"]