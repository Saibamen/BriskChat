using System.ComponentModel.DataAnnotations;

namespace TrollChat.DataAccess.Models
{
    public class EmailLogger : BaseEntity
    {
        [Required]
        public string Recipient { get; set; }

        [Required]
        public string Message { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public int FailureCount { get; set; }

        public string FailError { get; set; }

        public string FailErrorMessage { get; set; }
    }
}