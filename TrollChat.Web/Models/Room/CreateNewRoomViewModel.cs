using System.ComponentModel;

namespace BriskChat.Web.Models.Room
{
    public class CreateNewRoomViewModel
    {
        public bool IsPublic { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Purpose (optional)")]
        public string Description { get; set; }
    }
}