using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.ViewModels;

namespace Spotify.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            SearchVM model = new()
            {
                Artists = await _context.Artists.ToListAsync(),
                Songs = await _context.Songs.ToListAsync(),
                Albums = await _context.Albums.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Playlists = await _context.Playlist.ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Detail(int id, string category)
        {
            if (id == null && category == null) return BadRequest();

            CategoryDetailVM categoryDetailVM = new()
            {
                Category = await _context.Categories.Where(c => c.Id == id || c.Name == category).FirstOrDefaultAsync(),
                Artist = await _context.Artists.FirstOrDefaultAsync(),
                Albums = await _context.Albums.Include(m => m.Category).Include(a => a.Artist)
                .Where(a => a.CategoryId == id || a.Category.Name == category && !a.IsDeleted).ToListAsync()
            };

            return View(categoryDetailVM);
        }

        public async Task<IActionResult> Search(string searchText)
        {
            SearchVM model = null;

            if (!string.IsNullOrEmpty(searchText))
            {
                model = new()
                {
                    Artists = await _context.Artists.Where(m => m.FullName.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    ArtistSongs = await _context.ArtistSongs.Include(m => m.Artist).Include(m => m.Song).Where(m => m.Song.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    Albums = await _context.Albums.Include(m => m.Songs).Where(m => m.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    MusicPlaylists = await _context.MusicPlaylists.Include(m => m.Song).Include(m => m.Playlist).Where(m => m.Playlist.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync()
                };
            }
            else
            {
                model = new()
                {
                    Artists = await _context.Artists.Where(m => !m.IsDeleted).Take(4).ToListAsync(),
                    ArtistSongs = await _context.ArtistSongs.Include(m => m.Artist).Include(m => m.Song).Where(m => !m.IsDeleted).Take(4).ToListAsync(),
                    Albums = await _context.Albums.Include(m => m.Songs).Where(m => !m.IsDeleted).Take(4).ToListAsync(),
                    MusicPlaylists = await _context.MusicPlaylists.Include(m => m.Song).Include(m => m.Playlist).Where(m => !m.IsDeleted).Take(4).ToListAsync()
                };
            }

            return PartialView("_MainSearch", model);
        }

        public async Task<IActionResult> Filter(int id, string searchText)
        {
            SearchVM model = new();

            if (id == 0)
            {
                model = new()
                {
                    Artists = await _context.Artists.Where(m => m.FullName.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    ArtistSongs = await _context.ArtistSongs.Include(m => m.Artist).Include(m => m.Song).Where(m => m.Song.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    Albums = await _context.Albums.Include(m => m.Songs).Where(m => m.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync(),
                    MusicPlaylists = await _context.MusicPlaylists.Include(m => m.Song).Include(m => m.Playlist).Where(m => m.Playlist.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync()
                };

                return PartialView("_MainSearch", model);
            }
            else if (id == 1)
            {
                model.ArtistSongs = await _context.ArtistSongs.Include(m => m.Artist).Include(m => m.Song).ThenInclude(m => m.Album).Where(m => m.Song.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).ToListAsync();
                return PartialView("_Search", model);
            }
            else if (id == 2)
            {
                model.Artists = await _context.Artists.Where(m => m.FullName.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).ToListAsync();
                return PartialView("_Search", model);
            }
            else if (id == 3)
            {
                model.Albums = await _context.Albums.Include(m => m.Songs).Where(m => m.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync();
                return PartialView("_Search", model);
            }
            else if (id == 4)
            {
                model.MusicPlaylists = await _context.MusicPlaylists.Include(m => m.Song).Include(m => m.Playlist).Where(m => m.Playlist.Name.ToLower().Contains(searchText.ToLower()) && !m.IsDeleted).Take(4).ToListAsync();
                return PartialView("_Search", model);
            }

            return Ok();
        }
    }
}
