namespace BriskChat.BusinessLogic.Models
{
    public class RoleModel : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(Name.Trim()) && !string.IsNullOrWhiteSpace(Description.Trim());
        }
    }
}