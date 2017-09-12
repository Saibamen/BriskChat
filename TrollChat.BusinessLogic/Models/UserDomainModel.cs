namespace TrollChat.BusinessLogic.Models
{
    public class UserDomainModel : BaseModel
    {
        public UserModel User { get; set; }

        public DomainModel Domain { get; set; }

        public RoleModel Role { get; set; }
    }
}