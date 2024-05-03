using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Models;

namespace Spotify.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AppDbContext _context;

        public AlbumController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Detail(int id)
        {
            ViewBag.TotalSongCount = await _context.Songs.CountAsync(s => s.AlbumId == id);

            IQueryable<Album> albums = _context.Albums.AsNoTracking().AsQueryable();
            Album? album = await albums
               .Include(a => a.Songs.Take(5))
               .ThenInclude(a => a.ArtistSongs)
               .Include(a => a.Artist)
               .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null) return NotFound();
            ViewBag.OtherAlbums = await _context.Albums
    .Where(a => a.ArtistId == album.ArtistId && a.Id != id)
    .ToListAsync();

            return View(album);
        }
        public async Task<IActionResult> LoadMore(int albumId, int skip)
        {
            IQueryable<Album> albums = _context.Albums.AsNoTracking().AsQueryable();
            Album? album = await albums
               .Include(a => a.Songs.Skip(skip).Take(5))
               .ThenInclude(a => a.ArtistSongs)
               .Include(a => a.Artist)
               .FirstOrDefaultAsync(a => a.Id == albumId);

            return PartialView("_AlbumSongListPartial", album);
        }

    }
}
