using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class Room : BaseEntity
    {
        [Required]
        public User Owner { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Topic { get; set; }

        public int Customization { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPrivateConversation { get; set; }
    }
}