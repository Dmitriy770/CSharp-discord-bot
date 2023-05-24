using Xunit;

namespace DiscordBot.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(TestFixture))]
public class FixtureDefinition : ICollectionFixture<TestFixture>
{
}