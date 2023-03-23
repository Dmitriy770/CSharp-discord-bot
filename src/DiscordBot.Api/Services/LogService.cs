﻿using Discord;
using Discord.WebSocket;

namespace DiscordBot.Api.Services;

public class LogService
{
    private readonly ILogger<LogService> _logger;
    private readonly DiscordSocketClient _client;

    public LogService(ILogger<LogService> logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
        _client.Log += Log;
    }

    private Task Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Critical => LogLevel.Critical,
            _ => LogLevel.None
        };

        _logger.Log(logLevel, message.Exception, message.Message);
        
        return Task.CompletedTask;
    }
}