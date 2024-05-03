using Microsoft.AspNetCore.Identity;
using Spotify.Models;

namespace Spotify.ViewModels.UserViewModels
{
    public class ChanceRoleVM
    {
        public AppUser User { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
    }
}
