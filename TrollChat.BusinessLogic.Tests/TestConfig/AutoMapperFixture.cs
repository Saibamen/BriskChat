using BriskChat.BusinessLogic.Configuration.Implementations;

namespace BriskChat.BusinessLogic.Tests.TestConfig
{
    public class AutoMapperFixture
    {
        public AutoMapperFixture()
        {
            AutoMapperBuilder.Build();
        }
    }
}