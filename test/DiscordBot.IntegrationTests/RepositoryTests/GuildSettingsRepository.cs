using Bogus;
using DiscordBot.IntegrationTests.Fixtures;
using DiscordBot.VoiceManager.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DiscordBot.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class GuildSettingsRepository
{
    private readonly IGuildSettingsRepository _guildSettingsRepository;

    public GuildSettingsRepository(TestFixture fixture)
    {
        _guildSettingsRepository = fixture.GuildSettingsRepository;
    }

    [Fact]
    public async Task SetCreateVoiceChannel_SetCreateVoiceChannel_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var createVoiceChannelId = faker.Random.ULong();
        var bCreateVoiceChannelId = BitConverter.GetBytes(createVoiceChannelId);

        //Act
        await _guildSettingsRepository.SetCreateVoiceChannel(guildId, bCreateVoiceChannelId, CancellationToken.None);

        //Asserts
        var settings = await _guildSettingsRepository.Get(guildId, CancellationToken.None);
        BitConverter.ToUInt64(settings.CreateVoiceChannelId).Should().Be(createVoiceChannelId);
    }

    [Fact]
    public async Task Get_GetSettings_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var createVoiceChannelId = faker.Random.ULong();
        var bCreateVoiceChannelId = BitConverter.GetBytes(createVoiceChannelId);

        await _guildSettingsRepository.SetCreateVoiceChannel(guildId, bCreateVoiceChannelId, CancellationToken.None);

        //Act
        var settings = await _guildSettingsRepository.Get(guildId, CancellationToken.None);

        //Asserts
        BitConverter.ToUInt64(settings.CreateVoiceChannelId).Should().Be(createVoiceChannelId);
    }

    [Fact]
    public async Task Get_GetNull_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());

        //Act
        var settings = await _guildSettingsRepository.Get(guildId, CancellationToken.None);

        //Asserts
        settings.CreateVoiceChannelId.Should().BeNull();
    }

    [Fact]
    public async Task Delete_DeleteSettings_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = BitConverter.GetBytes(faker.Random.ULong());
        var createVoiceChannelId = BitConverter.GetBytes(faker.Random.ULong());

        await _guildSettingsRepository.SetCreateVoiceChannel(guildId, createVoiceChannelId, CancellationToken.None);

        //Act
        await _guildSettingsRepository.Delete(guildId, CancellationToken.None);

        //Asserts
        var settings = await _guildSettingsRepository.Get(guildId, CancellationToken.None);
        settings.CreateVoiceChannelId.Should().BeNull();
    }
}