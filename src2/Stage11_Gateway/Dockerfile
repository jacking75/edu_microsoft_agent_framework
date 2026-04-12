FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Stage11_Gateway/Stage11_Gateway.csproj", "Stage11_Gateway/"]
RUN dotnet restore "Stage11_Gateway/Stage11_Gateway.csproj"
COPY . .
WORKDIR "/src/Stage11_Gateway"
RUN dotnet build "Stage11_Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Stage11_Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stage11_Gateway.dll"]
