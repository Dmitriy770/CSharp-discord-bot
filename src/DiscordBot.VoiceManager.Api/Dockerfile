FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DiscordBot.Api/DiscordBot.Api.csproj", "src/DiscordBot.Api/"]
COPY ["src/DiscordBot.Dal/DiscordBot.Dal.csproj", "src/DiscordBot.Dal/"]
COPY ["src/DiscordBot.Bll/DiscordBot.Bll.csproj", "src/DiscordBot.Bll/"]
RUN dotnet restore "src/DiscordBot.Api/DiscordBot.Api.csproj"
COPY . .
WORKDIR "/src/src/DiscordBot.Api"
RUN dotnet build "DiscordBot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordBot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.Api.dll"]
