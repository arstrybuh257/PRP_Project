using System.ComponentModel.DataAnnotations;

namespace GainBargain.WEB.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}