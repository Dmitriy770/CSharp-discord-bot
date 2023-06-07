using Xunit;

namespace DiscordBot.VoiceManager.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(TestFixture))]
public class FixtureDefinition : ICollectionFixture<TestFixture>
{
}