using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.BusinessLogic.Models
{
    public class Message : BaseModel
    {
        public UserRoom UserRoom { get; set; }

        public int LastMessageForId { get; set; }

        public UserRoom LastMessageFor { get; set; }

        public string Text { get; set; }
    }
}