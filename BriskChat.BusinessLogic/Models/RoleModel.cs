namespace BriskChat.BusinessLogic.Models
{
    public class RoleModel : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description))
            {
                return false;
            }

            return !string.IsNullOrEmpty(Name.Trim()) && !string.IsNullOrEmpty(Description.Trim());
        }
    }
}