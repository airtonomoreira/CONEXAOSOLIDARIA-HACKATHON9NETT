FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DonationService/DonationService/DonationService.csproj", "src/DonationService/DonationService/"]
RUN dotnet restore "src/DonationService/DonationService/DonationService.csproj"
COPY . .
WORKDIR "/src/src/DonationService/DonationService"
RUN dotnet build "DonationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DonationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DonationService.dll"]
