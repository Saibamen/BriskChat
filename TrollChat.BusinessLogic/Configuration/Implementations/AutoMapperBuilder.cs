using System.Linq;
using AutoMapper;
using MimeKit;
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
                config.CreateMap<MimeMessage, EmailMessageModel>()
                    .ForMember(x => x.From, y => y.MapFrom(x => x.From.Mailboxes.FirstOrDefault().Address))
                    .ForMember(x => x.Recipient, y => y.MapFrom(x => x.To.Mailboxes.FirstOrDefault().Address))
                    .ForMember(x => x.Message, y => y.MapFrom(x => x.HtmlBody));

                config.CreateMap<UserRoomModel, UserRoom>().MaxDepth(2);
                config.CreateMap<UserRoom, UserRoomModel>().MaxDepth(2);

                config.CreateMap<Domain, DomainModel>().MaxDepth(1);
                config.CreateMap<DomainModel, Domain>().MaxDepth(1);

                config.CreateMissingTypeMaps = true;
            });
        }
    }
}