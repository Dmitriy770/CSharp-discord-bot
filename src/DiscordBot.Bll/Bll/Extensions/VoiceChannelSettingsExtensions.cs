using DiscordBot.Bll.Bll.Models;

namespace DiscordBot.Bll.Bll.Extensions;

public static class VoiceChannelSettingsExtensions
{
    public static void IsValid(this VoiceChannelSettingsModel settings)
    {
        settings.IsValidName();
        settings.IsValidLimit();
        settings.IsValidBitrate();
    }

    public static void IsValidName(this VoiceChannelSettingsModel settings)
    {
        if (settings.Name?.Length is <= 3 or >= 15)
        {
            throw new ArgumentException("Длина названия голосового канала должна быть в пределе от 3 до 15",
                nameof(settings.Name));
        }
    }

    public static void IsValidLimit(this VoiceChannelSettingsModel settings)
    {
        if (settings.Limit is < 1 or > 99)
        {
            throw new ArgumentException("Лимит пользователей должен быть в пределе от 1 до 99", nameof(settings.Limit));
        }
    }

    public static void IsValidBitrate(this VoiceChannelSettingsModel settings)
    {
        if (settings.Bitrate is < 8 or > 384)
        {
            throw new ArgumentException("Битрейт должен быть в пределе от 8 до 384", nameof(settings.Bitrate));
        }
    }
}