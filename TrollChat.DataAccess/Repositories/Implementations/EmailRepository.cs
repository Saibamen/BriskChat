using BriskChat.DataAccess.Context;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.DataAccess.Repositories.Implementations
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