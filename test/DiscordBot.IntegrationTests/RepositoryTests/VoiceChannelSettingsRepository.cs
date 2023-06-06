using Bogus;
using DiscordBot.IntegrationTests.Fixtures;
using DiscordBot.VoiceManager.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DiscordBot.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class VoiceChannelSettingsRepository
{
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;

    public VoiceChannelSettingsRepository(TestFixture fixture)
    {
        _voiceChannelSettingsRepository = fixture.VoiceChannelSettingsRepository;
    }

    [Fact]
    public async Task SetName_SetName_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var userId = BitConverter.GetBytes(faker.Random.ULong());
        var name = faker.Random.String(0, 15);

        //Act
        var settings = await _voiceChannelSettingsRepository.SetName(guildId, userId, name, CancellationToken.None);

        //Asserts
        settings.Name.Should().Be(name);
    }

    [Fact]
    public async Task SetName_Overwriting_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var userId = BitConverter.GetBytes(faker.Random.ULong());
        var usersLimit = faker.Random.Int();
        
        await _voiceChannelSettingsRepository.SetUsersLimit(guildId, userId, usersLimit, CancellationToken.None);
        await _voiceChannelSettingsRepository.SetName(guildId, userId, faker.Random.Utf16String(0, 15), CancellationToken.None);

        //Act
        var settings = await _voiceChannelSettingsRepository.SetName(
            guildId, userId, faker.Random.String(0,15), CancellationToken.None);

        //Asserts
        settings.UsersLimit.Should().Be(usersLimit);
    }
    
    [Fact]
    public async Task SetUsersLimit_SetUsersLimit_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var userId = BitConverter.GetBytes(faker.Random.ULong());
        var usersLimit = faker.Random.Int();

        //Act
        var settings = await _voiceChannelSettingsRepository.SetUsersLimit(guildId, userId, usersLimit, CancellationToken.None);

        //Asserts
        settings.UsersLimit.Should().Be(usersLimit);
    }
    
    [Fact]
    public async Task SetBitrate_Bitrate_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var userId = BitConverter.GetBytes(faker.Random.ULong());
        var bitrate = faker.Random.Int();

        //Act
        var settings = await _voiceChannelSettingsRepository.SetBitrate(guildId, userId, bitrate, CancellationToken.None);

        //Asserts
        settings.Bitrate.Should().Be(bitrate);
    }
    
    [Fact]
    public async Task Get_GetSettings_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = faker.Random.ULong();
        var bGuildId = BitConverter.GetBytes(guildId);
        var userId = faker.Random.ULong();
        var bUserId = BitConverter.GetBytes(userId);
        var name = faker.Random.Utf16String(0, 15);
        var usersLimit = faker.Random.Int();
        var bitrate = faker.Random.Int();

        await _voiceChannelSettingsRepository.SetName(bGuildId, bUserId, name, CancellationToken.None);
        await _voiceChannelSettingsRepository.SetUsersLimit(bGuildId, bUserId, usersLimit, CancellationToken.None);
        await _voiceChannelSettingsRepository.SetBitrate(bGuildId, bUserId, bitrate, CancellationToken.None);

        //Act
        var settings = await _voiceChannelSettingsRepository.Get(bGuildId, bUserId, CancellationToken.None);

        //Asserts
        BitConverter.ToUInt64(settings.GuildId).Should().Be(guildId);
        BitConverter.ToUInt64(settings.UserId).Should().Be(userId);
        settings.Name.Should().Be(name);
        settings.UsersLimit.Should().Be(usersLimit);
        settings.Bitrate.Should().Be(bitrate);
    }
    
    [Fact]
    public async Task Get_Default_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = faker.Random.ULong();
        var userId = faker.Random.ULong();

        //Act
        var settings = await _voiceChannelSettingsRepository.Get(
            BitConverter.GetBytes(guildId), BitConverter.GetBytes(userId), CancellationToken.None);

        //Asserts
        BitConverter.ToUInt64(settings.GuildId).Should().Be(guildId);
        BitConverter.ToUInt64(settings.UserId).Should().Be(userId);
        settings.Name.Should().Be(null);
        settings.UsersLimit.Should().Be(null);
        settings.Bitrate.Should().Be(null);
    }
    
    [Fact]
    public async Task Delete_Delete_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = faker.Random.ULong();
        var bGuildId = BitConverter.GetBytes(guildId);
        var userId = faker.Random.ULong();
        var bUserId = BitConverter.GetBytes(userId);
        var name = faker.Random.Utf16String(0, 15);
        var usersLimit = faker.Random.Int();
        var bitrate = faker.Random.Int();

        await _voiceChannelSettingsRepository.SetName(bGuildId, bUserId, name, CancellationToken.None);
        await _voiceChannelSettingsRepository.SetUsersLimit(bGuildId, bUserId, usersLimit, CancellationToken.None);
        await _voiceChannelSettingsRepository.SetBitrate(bGuildId, bUserId, bitrate, CancellationToken.None);

        //Act
        await _voiceChannelSettingsRepository.Delete(bGuildId, bUserId, CancellationToken.None);

        //Asserts
        var settings = await _voiceChannelSettingsRepository.Get(bGuildId, bUserId, CancellationToken.None);
        
        BitConverter.ToUInt64(settings.GuildId).Should().Be(guildId);
        BitConverter.ToUInt64(settings.UserId).Should().Be(userId);
        settings.Name.Should().Be(null);
        settings.UsersLimit.Should().Be(null);
        settings.Bitrate.Should().Be(null);
    }
    
    [Fact]
    public async Task Delete_DeleteMissingLine_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = faker.Random.ULong();
        var bGuildId = BitConverter.GetBytes(guildId);
        
        var userId = faker.Random.ULong();
        var bUserId = BitConverter.GetBytes(userId);

        //Act
        await _voiceChannelSettingsRepository.Delete(bGuildId, bUserId, CancellationToken.None);

        //Asserts
        var settings = await _voiceChannelSettingsRepository.Get(bGuildId, bUserId, CancellationToken.None);
        
        BitConverter.ToUInt64(settings.GuildId).Should().Be(guildId);
        BitConverter.ToUInt64(settings.UserId).Should().Be(userId);
        settings.Name.Should().Be(null);
        settings.UsersLimit.Should().Be(null);
        settings.Bitrate.Should().Be(null);
    }
}