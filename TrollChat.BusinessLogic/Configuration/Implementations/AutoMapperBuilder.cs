using AutoMapper;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Models;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public static class AutoMapperBuilder
    {
        public static void Build()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<User, UserModel>();
                config.CreateMap<User, UserModel>().ReverseMap();

                config.CreateMap<UserToken, UserTokenModel>();
                config.CreateMap<UserToken, UserTokenModel>().ReverseMap();

                config.CreateMap<Room, RoomModel>();
                config.CreateMap<Room, RoomModel>().ReverseMap();
            });
        }
    }
}