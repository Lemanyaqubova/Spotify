using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.AccountViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Prompt = "Enter your email")]
        public string Email { get; set; }
    }
}
