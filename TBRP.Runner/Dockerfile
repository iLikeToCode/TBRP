FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TBRP.Runner/TBRP.Runner.csproj", "TBRP.Runner/"]
COPY ["TBRP.DiscordBot/TBRP.DiscordBot.csproj", "TBRP.DiscordBot/"]
COPY ["TBRP.DB/TBRP.DB.csproj", "TBRP.DB/"]
COPY ["TBRP.Api/TBRP.Api.csproj", "TBRP.Api/"]
RUN dotnet restore "TBRP.Runner/TBRP.Runner.csproj"
COPY . .
WORKDIR "/src/TBRP.Runner"
RUN dotnet build "./TBRP.Runner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TBRP.Runner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TBRP.Runner.dll"]
