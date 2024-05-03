using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Extensions;
using Spotify.Models;
using Spotify.ViewModels.ArtistViewModels;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Moderator, Admin")]
    public class ArtistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ArtistController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _context = appDbContext;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Artist> artists = await _context.Artists
               .Include(a => a.Albums)
               .Include(p => p.ArtistPositions)
               .ThenInclude(p => p.Position)
               .ToListAsync();
            return View(artists);
        }
        public async Task<IActionResult> Detaill(int? id)
        {
            if (id == null) return NotFound();
            Artist artist = _context.Artists.Include(c => c.Albums).Include(a => a.ArtistPositions).ThenInclude(p => p.Position).SingleOrDefault(c => c.Id == id);
            List<Album> albums = await _context.Albums.Where(m => m.ArtistId == id).ToListAsync();
            if (artist == null) return NotFound();

            return View(artist);
        }
        public async Task<IActionResult> Create()
        {
            ArtistCreateVM model = new ArtistCreateVM
            {
                Positions = await _context.Positions.ToListAsync(),

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ArtistCreateVM artistCreateVM)
        {
            ArtistCreateVM model = new ArtistCreateVM
            {
                Positions = await _context.Positions.ToListAsync(),

            };
            if (!ModelState.IsValid) return View(model);
            if (!ModelState.IsValid) return View(model);
            Artist newArtist = new Artist();
            newArtist.ArtistPositions = new List<ArtistPosition>();
            if (artistCreateVM.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please Input Image");
                return View();

            }
            if (!artistCreateVM.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "please input only image");
                return View();
            }
            if (artistCreateVM.AboutImg == null)
            {
                ModelState.AddModelError("AboutImg", "Please Input Image");
                return View();

            }
            if (!artistCreateVM.AboutImg.IsImage())
            {
                ModelState.AddModelError("AboutImg", "please input only image");
                return View();
            }

            foreach (int id in artistCreateVM.PositionIds)
            {
                ArtistPosition position = new ArtistPosition()
                {
                    PositionId = id,
                    Artist = newArtist
                };
                newArtist.ArtistPositions.Add(position);
            }
            newArtist.FullName = artistCreateVM.FullName;
            newArtist.ImageUrl = artistCreateVM.Photo.SaveImage(_env, "assets/images", artistCreateVM.Photo.FileName);
            newArtist.AboutImg = artistCreateVM.AboutImg.SaveImage(_env, "assets/images", artistCreateVM.AboutImg.FileName);

            await _context.Artists.AddAsync(newArtist);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            if (id == 0) return BadRequest();

            ArtistUpdateVM model = UpdatedArtist(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ArtistUpdateVM artistUpdateVM)
        {
            if (id == 0) return BadRequest();
            ArtistUpdateVM model = UpdatedArtist(id);
            if (!ModelState.IsValid) return View(model);

            Artist dbArtist = _context.Artists.Include(p => p.ArtistPositions).FirstOrDefault(p => p.Id == id)!;
            if (dbArtist == null)
                return NotFound();

            dbArtist.FullName = artistUpdateVM.FullName;
            var removableArtist = dbArtist.ArtistPositions.Where(p => !artistUpdateVM.PositionIds.Contains(p.PositionId)).ToList();

            foreach (var item in removableArtist)
            {
                dbArtist.ArtistPositions.Remove(item);
            }

            foreach (var positionId in artistUpdateVM.PositionIds)
            {
                if (!dbArtist.ArtistPositions.Any(p => p.PositionId == positionId))
                {
                    ArtistPosition newposition = new ArtistPosition
                    {
                        PositionId = positionId,
                        ArtistId = dbArtist.Id
                    };
                    dbArtist.ArtistPositions.Add(newposition);
                }
            }
            if (artistUpdateVM.Photo != null)
            {
                if (artistUpdateVM.Photo == null)
                {
                    ModelState.AddModelError("Photo", "bosh qoyma");
                }
                if (!artistUpdateVM.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "only image select");
                }
                string fullPath = Path.Combine(_env.WebRootPath, "assets/images", dbArtist.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                dbArtist.ImageUrl = artistUpdateVM.Photo.SaveImage(_env, "assets/images", artistUpdateVM.Photo.FileName);
                _context.SaveChanges();
            }
            if (artistUpdateVM.AboutPhoto != null)
            {
                if (artistUpdateVM.AboutPhoto == null)
                {
                    ModelState.AddModelError("AboutPhoto", "bosh qoyma");
                }
                if (!artistUpdateVM.AboutPhoto.IsImage())
                {
                    ModelState.AddModelError("AboutPhoto", "only image select");
                }
                string fullPath = Path.Combine(_env.WebRootPath, "assets/images", dbArtist.AboutImg);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                dbArtist.AboutImg = artistUpdateVM.AboutPhoto.SaveImage(_env, "assets/images", artistUpdateVM.AboutPhoto.FileName);
                _context.SaveChanges();
            }


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        ArtistUpdateVM UpdatedArtist(int id)
        {
            var artist = _context.Artists
                .Include(p => p.ArtistPositions)
                .FirstOrDefault(p => p.Id == id);

            if (artist == null)
            {

                return new ArtistUpdateVM();
            }


            ArtistUpdateVM model = _context.Artists.Select(s => new ArtistUpdateVM()
            {
                Id = s.Id,
                FullName = s.FullName,
                ImageUrl = s.ImageUrl,
                AboutImg = s.AboutImg,
                ArtistPositions = s.ArtistPositions.ToList()
            }).FirstOrDefault(s => s.Id == id);
            model.PositionIds = artist.ArtistPositions.Select(ap => ap.PositionId).ToList();
            model.Positions = _context.Positions.ToList();

            return model;
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Artist? artist = await _context.Artists.FirstOrDefaultAsync(s => s.Id == id);
            if (artist == null) NotFound();
            else
            {
                _context.Remove(artist);
                _context.SaveChanges();
                string fullpath = Path.Combine(_env.WebRootPath, "assets/images", artist.ImageUrl);
                string aboutImg = Path.Combine(_env.WebRootPath, "assets/images", artist.AboutImg);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                if (System.IO.File.Exists(aboutImg))
                {
                    System.IO.File.Delete(aboutImg);
                }

            };
            return RedirectToAction("Index");
        }

    }
}
