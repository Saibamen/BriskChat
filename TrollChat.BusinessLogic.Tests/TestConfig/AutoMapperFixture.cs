using TrollChat.BusinessLogic.Configuration.Implementations;

namespace TrollChat.BusinessLogic.Tests.TestConfig
{
    public class AutoMapperFixture
    {
        public AutoMapperFixture()
        {
            AutoMapperBuilder.Build();
        }
    }
}