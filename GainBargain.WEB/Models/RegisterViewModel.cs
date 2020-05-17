using System.ComponentModel.DataAnnotations;

namespace GainBargain.WEB.Models
{
    public class RegisterViewModel
    {
        private int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}