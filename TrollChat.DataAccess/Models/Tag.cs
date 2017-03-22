using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class Tag : BaseEntity
    {
        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Description { get; set; }
    }
}