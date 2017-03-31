using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class Message : BaseEntity
    {
        [Required]
        public UserRoom UserRoom { get; set; }

        [ForeignKey(nameof(LastMessageFor))]
        public int LastMessageForId { get; set; }

        public UserRoom LastMessageFor { get; set; }

        [Required]
        public string Text { get; set; }
    }
}