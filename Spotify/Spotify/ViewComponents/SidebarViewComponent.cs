using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.ViewModels;
using System.Security.Claims;

namespace Spotify.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _ctx;

        public SidebarViewComponent(AppDbContext context, IHttpContextAccessor ctx)
        {
            _context = context;
            _ctx = ctx;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = _ctx.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            int songCount = await _context.WishlistItems.Where(m => m.Wishlist.AppUserId == userId && !m.IsDeleted).CountAsync();

            SidebarVM model = new() { SongCount = songCount };

            return await Task.FromResult(View(model));
        }
    }
}
