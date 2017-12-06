using Xunit;

namespace BriskChat.BusinessLogic.Tests.TestConfig
{
    [CollectionDefinition("mapper")]
    public class BaseFixture : ICollectionFixture<AutoMapperFixture>
    {
    }
}