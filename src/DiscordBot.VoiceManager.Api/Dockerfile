FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DiscordBot.VoiceManager.Api/DiscordBot.VoiceManager.Api.csproj", "src/DiscordBot.VoiceManager.Api/"]
COPY ["src/DiscordBot.VoiceManager.Infrastructure/DiscordBot.VoiceManager.Infrastructure.csproj", "src/DiscordBot.VoiceManager.Infrastructure/"]
COPY ["src/DiscordBot.VoiceManager.Domain/DiscordBot.VoiceManager.Domain.csproj", "src/DiscordBot.VoiceManager.Domain/"]
RUN dotnet restore "src/DiscordBot.VoiceManager.Api/DiscordBot.VoiceManager.Api.csproj"
COPY . .
WORKDIR "/src/src/DiscordBot.VoiceManager.Api"
RUN dotnet build "DiscordBot.VoiceManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscordBot.VoiceManager.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.VoiceManager.Api.dll"]
