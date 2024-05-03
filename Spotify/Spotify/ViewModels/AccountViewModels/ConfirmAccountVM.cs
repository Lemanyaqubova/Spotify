using Spotify.Models;
using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.AccountViewModels
{
    public class ConfirmAccountVM
    {
        public string Email { get; set; }
        [Required]
        public string? OTP { get; set; }
        public AppUser? AppUser { get; set; }

        public ConfirmAccountVM()
        {
            AppUser = new();
        }
    }
}
