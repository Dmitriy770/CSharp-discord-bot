using Bogus;
using DiscordBot.IntegrationTests.Fixtures;
using DiscordBot.VoiceManager.Domain.Entities;
using DiscordBot.VoiceManager.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace DiscordBot.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class VoiceChannelCacheRepository
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;

    public VoiceChannelCacheRepository(TestFixture fixture)
    {
        _voiceChannelCacheRepository = fixture.VoiceChannelCacheRepository;
    }

    [Fact]
    public async Task Add_AddCache_Success()
    {
        // Arrange
        var faker = new Faker();

        var voiceChannelCache = new VoiceChannelCacheEntity(
            faker.Random.ULong(),
            faker.Random.ULong(),
            faker.Random.ULong()
        );

        //Act
        await _voiceChannelCacheRepository.Add(voiceChannelCache);

        //Asserts
        var cache = await _voiceChannelCacheRepository.Get(
            voiceChannelCache.GuildId, voiceChannelCache.Id);
        cache.Should().Be(voiceChannelCache);
    }
    
    [Fact]
    public async Task Get_GetCache_Success()
    {
        // Arrange
        var faker = new Faker();

        var voiceChannelCache = new VoiceChannelCacheEntity(
            faker.Random.ULong(),
            faker.Random.ULong(),
            faker.Random.ULong()
        );
        await _voiceChannelCacheRepository.Add(voiceChannelCache);

        //Act
        var cache = await _voiceChannelCacheRepository.Get(
            voiceChannelCache.GuildId, voiceChannelCache.Id);
        
        //Asserts
        cache.Should().Be(voiceChannelCache);
    }
    
    [Fact]
    public async Task Get_GetEmpty_Success()
    {
        // Arrange
        var faker = new Faker();

        var guildId = faker.Random.ULong();
        var voiceChannelId = faker.Random.ULong();

        //Act
        var cache = await _voiceChannelCacheRepository.Get(guildId, voiceChannelId);
        
        //Asserts
        cache.GuildId.Should().Be(guildId);
        cache.Id.Should().Be(voiceChannelId);
        cache.OwnerId.Should().BeNull();
    }
    
    [Fact]
    public async Task Delete_DeleteCache_Success()
    {
        // Arrange
        var faker = new Faker();

        var voiceChannelCache = new VoiceChannelCacheEntity(
            faker.Random.ULong(),
            faker.Random.ULong(),
            faker.Random.ULong()
        );
        await _voiceChannelCacheRepository.Add(voiceChannelCache);

        //Act
        await _voiceChannelCacheRepository.Delete(
            voiceChannelCache.GuildId, voiceChannelCache.Id);

        //Asserts
        var cache = await _voiceChannelCacheRepository.Get(
            voiceChannelCache.GuildId, voiceChannelCache.Id);
        cache.GuildId.Should().Be(voiceChannelCache.GuildId);
        cache.Id.Should().Be(voiceChannelCache.Id);
        cache.OwnerId.Should().BeNull();
    }
}