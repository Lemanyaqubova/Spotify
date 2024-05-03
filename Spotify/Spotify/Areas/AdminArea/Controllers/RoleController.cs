using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            IdentityResult result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                return RedirectToAction("Error");
            }

            return RedirectToAction("Index");
        }
    }
}
