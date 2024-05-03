using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Models;
using Spotify.ViewModels;

namespace FoodFunBackend.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    //[Authorize(Roles = "SuperAdmin, Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(AppDbContext appDbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            DashboardVM dashboardVM = new()
            {
                Artists = await _context.Artists.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Albums = await _context.Albums.ToListAsync(),
                Songs = await _context.Songs.ToListAsync(),
                Positions = await _context.Positions.ToListAsync(),
                AppUsers = await _context.Users.ToListAsync(),
                IdentityRoles = await _roleManager.Roles.ToListAsync(),

            };

            return View(dashboardVM);
        }
    }
}
