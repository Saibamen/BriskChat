using Xunit;

namespace TrollChat.BusinessLogic.Tests.TestConfig
{
    [CollectionDefinition("mapper")]
    public class BaseFixture : ICollectionFixture<AutoMapperFixture>
    {
    }
}