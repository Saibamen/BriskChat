using TrollChat.BusinessLogic.Configuration.Implementations;

namespace TrollChat.BusinessLogic.Tests
{
    public class AutoMapperFixture
    {
        public AutoMapperFixture()
        {
            AutoMapperBuilder.Build();
        }
    }
}