using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class EmailRepository : GenericRepository<EmailLogger>, IEmailRepository
    {
        public EmailRepository(ITrollChatDbContext context)
            : base(context)
        {
        }

        public override void Delete(EmailLogger entity)
        {
            context.Set<EmailLogger>().Remove(entity);
        }
    }
}