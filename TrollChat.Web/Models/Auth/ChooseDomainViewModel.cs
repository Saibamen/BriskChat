using System.ComponentModel.DataAnnotations;

namespace TrollChat.Web.Models.Auth
{
    public class ChooseDomainViewModel
    {
        [Required]
        [Display(Name = "Domain name")]
        public string DomainName { get; set; }
    }
}