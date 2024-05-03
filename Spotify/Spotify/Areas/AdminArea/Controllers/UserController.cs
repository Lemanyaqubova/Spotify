using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Models;
using Spotify.ViewModels.UserViewModels;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
        public async Task<IActionResult> Detail(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();


            return View(new UserDetailVM
            {
                User = user,
                UserRoles = await _userManager.GetRolesAsync(user)
            });
        }
        public IActionResult BlockedUser()
        {
            return View(_userManager.Users.Where(u => !u.IsActive));
        }
        public async Task<IActionResult> Active(string id, bool IsActive)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user.IsActive = true)
            {
                user.IsActive = false;
            }

            return View(_userManager.Users.ToList());
        }
        public async Task<IActionResult> EditRole(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(new ChanceRoleVM
            {
                User = user,
                UserRoles = await _userManager.GetRolesAsync(user),
                AllRoles = _roleManager.Roles.ToList()
            });

        }
        [HttpPost]
        public async Task<IActionResult> EditRole(string id, List<string> roles)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, roles);
            return RedirectToAction("index", "user");

        }
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();


            IdentityResult result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {

                return RedirectToAction("Error");
            }
            await _signInManager.SignOutAsync();


            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            if (user.IsActive)
            {
                user.IsActive = false;
            }
            else
            {
                user.IsActive = true;
            }

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return RedirectToAction("Error");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
