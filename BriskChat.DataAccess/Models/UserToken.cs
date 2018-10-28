using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BriskChat.DataAccess.Models
{
    public class UserToken : BaseEntity
    {
        [Required]
        public User User { get; set; }

        [Column(TypeName = "NVARCHAR(256)")]
        public string SecretToken { get; set; }

        [Required]
        public DateTime? SecretTokenTimeStamp { get; set; }
    }
}