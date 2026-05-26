FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CampaignService/CampaignService/CampaignService.csproj", "src/CampaignService/CampaignService/"]
RUN dotnet restore "src/CampaignService/CampaignService/CampaignService.csproj"
COPY . .
WORKDIR "/src/src/CampaignService/CampaignService"
RUN dotnet build "CampaignService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CampaignService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CampaignService.dll"]
