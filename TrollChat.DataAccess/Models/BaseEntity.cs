using System;
using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class BaseEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}