using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class DomainRoom : BaseEntity
    {
        [Required]
        public Domain Domain { get; set; }

        [Required]
        public Room Room { get; set; }
    }
}