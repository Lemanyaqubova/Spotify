using Spotify.Models;

namespace Spotify.ViewModels.UserViewModels
{
    public class UserDetailVM
    {
        public AppUser User { get; set; }
        public IList<string> UserRoles { get; set; }
        public string Email { get; set; }

    }
}
