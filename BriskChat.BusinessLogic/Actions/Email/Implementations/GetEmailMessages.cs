﻿using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Email.Interfaces;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.Email.Implementations
{
    public class GetEmailMessages : IGetEmailMessages
    {
        private readonly IEmailRepository _emailRepository;

        public GetEmailMessages(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public List<EmailMessage> Invoke(int count)
        {
            return _emailRepository.FindBy(x => x.Message != null).Take(count).ToList();
        }
    }
}