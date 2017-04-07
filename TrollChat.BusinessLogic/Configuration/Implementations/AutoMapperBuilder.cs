using AutoMapper;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public static class AutoMapperBuilder
    {
        public static void Build()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMissingTypeMaps = true;
            });
        }
    }
}