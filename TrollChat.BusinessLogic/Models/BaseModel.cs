using System;

namespace TrollChat.BusinessLogic.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}