using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrollChat.DataAccess.Models
{
    public class User : BaseEntity
    {
        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public String Email { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(256)")]
        public String PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(128)")]
        public string PasswordSalt { get; set; }

        public DateTime? EmailConfirmedOn { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public String Name { get; set; }      

        public DateTime? LockedOn { get; set; }
    }
}