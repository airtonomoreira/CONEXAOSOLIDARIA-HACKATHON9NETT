FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/TransparencyService/TransparencyService/TransparencyService.csproj", "src/TransparencyService/TransparencyService/"]
RUN dotnet restore "src/TransparencyService/TransparencyService/TransparencyService.csproj"
COPY . .
WORKDIR "/src/src/TransparencyService/TransparencyService"
RUN dotnet build "TransparencyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransparencyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransparencyService.dll"]
