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
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(Name.Trim());
        }
    }
}