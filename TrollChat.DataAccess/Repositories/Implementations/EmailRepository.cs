using TrollChat.DataAccess.Context;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.DataAccess.Repositories.Implementations
{
    public class EmailRepository : GenericRepository<EmailMessage>, IEmailRepository
    {
        public EmailRepository(ITrollChatDbContext context)
            : base(context)
        {
        }

        public override void Delete(EmailMessage entity)
        {
            Context.Set<EmailMessage>().Remove(entity);
        }
    }
}