using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Extensions;
using Spotify.Models;
using Spotify.ViewModels.AlbumViewModels;
namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Moderator, Admin")]
    public class AlbumController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AlbumController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _context = appDbContext;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Album> album = await _context.Albums
                .Include(a => a.Artist)
                .Include(c => c.Category)
                .Include(s => s.Songs)
                .ToListAsync();
            return View(album);
        }
        public async Task<IActionResult> Detaill(int? id)
        {
            if (id == null) return NotFound();
            Album album = _context.Albums.Include(c => c.Category).Include(a => a.Artist).SingleOrDefault(c => c.Id == id);
            List<Song> albumSong = await _context.Songs.Where(m => m.AlbumId == id).ToListAsync();
            if (album == null) return NotFound();

            return View(album);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateVM albumCreateVM)
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "FullName");
            if (albumCreateVM.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please Input Image");
                return View();

            }
            if (!albumCreateVM.Photo.IsImage())
            {
                ModelState.AddModelError("photo", "please input only image");
                return View();
            }
            Album newAlbum = new()
            {
                Name = albumCreateVM.Name,
                ImageUrl = albumCreateVM.Photo.SaveImage(_env, "assets/images", albumCreateVM.Photo.FileName),
                ArtistId = albumCreateVM.ArtistId,
                CategoryId = albumCreateVM.CategoryId,
                CreateDate = albumCreateVM.CreatedTime
            };
            await _context.Albums.AddAsync(newAlbum);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "FullName");
            if (id == null) return NotFound();
            Album? album = await _context.Albums.FirstOrDefaultAsync(c => c.Id == id);
            if (album == null) return NotFound();
            return View(new AlbumUpdateVM
            {
                ImageUrl = album.ImageUrl,
                Name = album.Name,
                CreatedTime = album.CreateDate,
                CategoryId = album.CategoryId,
                ArtistId = album.ArtistId
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, AlbumUpdateVM albumUpdateVM)
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "FullName");
            if (id == null) return NotFound();
            Album? album = await _context.Albums.FirstOrDefaultAsync(c => c.Id == id);
            if (album == null) return NotFound();
            if (albumUpdateVM.Photo != null)
            {
                if (albumUpdateVM.Photo == null)
                {
                    ModelState.AddModelError("Photo", "bosh qoyma");
                    return View(albumUpdateVM = new() { ImageUrl = album.ImageUrl });
                }
                if (!albumUpdateVM.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "only image select");
                    return View(albumUpdateVM = new() { ImageUrl = album.ImageUrl });
                }
                string fullPath = Path.Combine(_env.WebRootPath, "assets/images", album.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                album.ImageUrl = albumUpdateVM.Photo.SaveImage(_env, "assets/images", albumUpdateVM.Photo.FileName);
                _context.SaveChanges();
            }

            album.Name = albumUpdateVM.Name;
            album.CreateDate = albumUpdateVM.CreatedTime;
            album.ArtistId = albumUpdateVM.ArtistId;
            album.CategoryId = albumUpdateVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Album? album = await _context.Albums.FirstOrDefaultAsync(s => s.Id == id);
            if (album == null) NotFound();



            else
            {
                _context.Remove(album);
                _context.SaveChanges();
                string fullpath = Path.Combine(_env.WebRootPath, "assets/images", album.ImageUrl);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
            };
            return RedirectToAction("Index");
        }


    }
}

