using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class Message : BaseEntity
    {
        [Required]
        public UserRoom UserRoom { get; set; }

        [Required]
        public string Text { get; set; }
    }
}