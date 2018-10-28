using System.Collections.Generic;

namespace BriskChat.BusinessLogic.Models
{
    public class DomainModel : BaseModel
    {
        public string Name { get; set; }

        public UserModel Owner { get; set; }

        public List<UserModel> Users { get; set; }
        public List<RoomModel> Rooms { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            return !string.IsNullOrEmpty(Name.Trim());
        }
    }
}