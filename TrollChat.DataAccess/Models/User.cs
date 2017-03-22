using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class User : BaseEntity
    {
        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public string PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(128)")]
        public string PasswordSalt { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string Name { get; set; }      

        public DateTime? LockedOn { get; set; }
    }
}