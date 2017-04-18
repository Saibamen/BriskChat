using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrollChat.BusinessLogic.Models
{
    public class DomainModel : BaseModel
    {
        public string Name { get; set; }

        public UserModel Owner { get; set; }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(Name);
        }
    }
}