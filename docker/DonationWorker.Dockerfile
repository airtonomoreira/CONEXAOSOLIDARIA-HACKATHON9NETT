FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DonationWorker/DonationWorker/DonationWorker.csproj", "src/DonationWorker/DonationWorker/"]
RUN dotnet restore "src/DonationWorker/DonationWorker/DonationWorker.csproj"
COPY . .
WORKDIR "/src/src/DonationWorker/DonationWorker"
RUN dotnet build "DonationWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DonationWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DonationWorker.dll"]
