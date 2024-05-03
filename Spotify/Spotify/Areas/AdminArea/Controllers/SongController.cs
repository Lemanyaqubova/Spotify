using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Extensions;
using Spotify.Helpers;
using Spotify.Models;
using Spotify.ViewModels.SongViewModels;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Moderator, Admin")]
    public class SongController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SongController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _context = appDbContext;
            _env = env;
        }
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<Song> songs = await _context.Songs
                .Where(s => !s.IsDeleted)
                .Include(s => s.Album)
                .Include(s => s.Category)
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();

            List<SongListVM> mapDatas = GetMapDatas(songs);

            int count = await GetPageCount(take);

            PaginationVM<SongListVM> result = new(mapDatas, page, count);

            return View(result);
        }
        private async Task<int> GetPageCount(int take)
        {
            int songCount = await _context.Songs.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)songCount / take);
        }

        private static List<SongListVM> GetMapDatas(List<Song> songs)
        {
            List<SongListVM> songList = new();

            foreach (var song in songs)
            {
                SongListVM newSong = new()
                {
                    Id = song.Id,
                    Name = song.Name,
                    Category = song.Category.Name,
                    ImageUrl = song.ImageUrl,
                    Color = song.Color,
                    Path = song.Path,
                    CreateDate = song.CreateDate,
                    AlbumName = song.Album.Name
                };

                songList.Add(newSong);
            }
            return songList;
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Song song = await _context.Songs
            .Where(m => !m.IsDeleted && m.Id == id)
            .Include(m => m.ArtistSongs)
                .Include(m => m.MusicPlaylists)
                .Include(m => m.Category)
                .Include(m => m.Album)
                .FirstOrDefaultAsync();

            List<ArtistSong> artistSong = await _context.ArtistSongs.Where(m => m.SongId == id).ToListAsync();
            List<Artist> artists = new List<Artist>();
            foreach (var size in artistSong)
            {
                Artist dbArtist = await _context.Artists.Where(m => m.Id == size.ArtistId).FirstOrDefaultAsync();
                artists.Add(dbArtist);
            }

            if (song == null) return NotFound();

            var data = await GetArtistAsync();

            SongDetailVm songDetail = new()
            {
                Id = song.Id,
                Name = song.Name,
                Category = song.Category.Name,
                ImageUrl = song.ImageUrl,
                Color = song.Color,
                Path = song.Path,
                CreateDate = song.CreateDate,
                AlbumName = song.Album.Name,
                Rating = song.PointRayting,
                Artists = artists,
            };

            return View(songDetail);
        }
        private async Task<SelectList> GetArtistAsync()
        {
            List<Artist> artists = await _context.Artists.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(artists, "Id", "FullName");
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.Artists = await GetArtistAsync();
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongCreateVM songCreateVM)
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            if (!ModelState.IsValid)
            {
                ViewBag.Artists = await GetArtistAsync();
                ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(songCreateVM);
            }

            if (songCreateVM.Audio == null)
            {
                ModelState.AddModelError("Audio", "Uploaded file is empty");
                return View(songCreateVM);
            }

            if (!songCreateVM.Audio.IsAudio())
            {
                ViewBag.Artists = await GetArtistAsync();
                ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                ModelState.AddModelError("Audio", "Should be an audio file with mp3 extension");
                return View(songCreateVM);
            }

            if (songCreateVM.Audio.CheckAudioSize(20000))
            {
                ViewBag.Artists = await GetArtistAsync();
                ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                ModelState.AddModelError("Audio", "Song should not be bigger than 20mb");
                return View(songCreateVM);
            }
            if (songCreateVM.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please Input Image");
                return View();

            }
            if (!songCreateVM.Photo.IsImage())
            {
                ViewBag.Artists = await GetArtistAsync();
                ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                ModelState.AddModelError("Photo", "please input only image");
                return View();
            }

            Song newSong = new()
            {
                ImageUrl = songCreateVM.Photo?.SaveImage(_env, "assets/images", songCreateVM.Photo.FileName),
                Name = songCreateVM.Name,
                CreateDate = songCreateVM.CreateDate,
                CategoryId = songCreateVM.CategoryId,
                Color = songCreateVM.Color,
                AlbumId = songCreateVM.AlbumId,
                Path = songCreateVM.Audio.SaveAudio(_env, "assets/music", songCreateVM.Audio.FileName)
            };

            await _context.Songs.AddAsync(newSong);
            await _context.SaveChangesAsync();

            foreach (int artistId in songCreateVM.ArtistIds)
            {
                var artist = _context.Artists.Include(a => a.Albums).FirstOrDefault(a => a.Id == artistId);

                if (artist == null || !artist.Albums.Any())
                {
                    ModelState.AddModelError("", $"Artist with ID {artistId} does not have any albums");
                    return View(songCreateVM);
                }

                await _context.ArtistSongs.AddAsync(new ArtistSong { ArtistId = artistId, SongId = newSong.Id });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetAlbums(int artistId)
        {
            var artist = _context.Artists
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.Id == artistId);

            if (artist == null)
            {
                return NotFound();
            }

            var albums = artist.Albums.Select(a => new { a.Id, a.Name }).ToList();
            return Json(albums);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.Where(m => !m.IsDeleted).ToListAsync(), "Id", "FullName");

            if (id == 0) return BadRequest();

            SongUpdateVM model = UpdateSong(id);
            if (model == null) return NotFound();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, SongUpdateVM songUpdateVM)
        {
            if (id == 0) return BadRequest();
            SongUpdateVM model = UpdateSong(id);
            ViewBag.Category = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Artists = new SelectList(await _context.Artists.Where(m => !m.IsDeleted).ToListAsync(), "Id", "FullName");

            if (model == null) return NotFound();
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            Song dbSong = _context.Songs.Include(s => s.ArtistSongs)
                                                .Include(s => s.Category)
                                                .FirstOrDefault(p => p.Id == id)!;
            if (dbSong == null) return NotFound();
            var removableArtist = dbSong.ArtistSongs.Where(s => !songUpdateVM.ArtistsIds.Contains(s.ArtistId)).ToList();

            foreach (var artist in removableArtist)
            {
                dbSong.ArtistSongs.Remove(artist);
            }

            foreach (var artistId in songUpdateVM.ArtistsIds)
            {
                if (!dbSong.ArtistSongs.Any(s => s.ArtistId == artistId))
                {
                    dbSong.ArtistSongs.Add(new ArtistSong { ArtistId = artistId });
                }
            }
            if (dbSong == null) return NotFound();
            if (songUpdateVM.Photo != null)
            {
                if (!songUpdateVM.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select only an image file.");
                    return View(model);
                }

                string fullPath = Path.Combine(_env.WebRootPath, "assets/images", dbSong.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                dbSong.ImageUrl = songUpdateVM.Photo.SaveImage(_env, "assets/images", songUpdateVM.Photo.FileName);
            }

            if (songUpdateVM.Audio != null)
            {
                if (!songUpdateVM.Audio.IsAudio())
                {
                    ModelState.AddModelError("Audio", "Please select only an audio file with mp3 extension.");
                    return View(model);
                }

                if (songUpdateVM.Audio.CheckAudioSize(20000))
                {
                    ModelState.AddModelError("Audio", "Song should not be bigger than 20mb.");
                    return View(model);
                }

                dbSong.Path = songUpdateVM.Audio.SaveAudio(_env, "assets/music", songUpdateVM.Audio.FileName);
            }

            dbSong.Name = songUpdateVM.Name;
            dbSong.Color = songUpdateVM.Color;
            dbSong.CategoryId = songUpdateVM.CategoryId;
            dbSong.CreateDate = songUpdateVM.CreateDate;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        SongUpdateVM UpdateSong(int id)
        {
            SongUpdateVM model = _context.Songs.Include(s => s.Category)
                .Include(s => s.ArtistSongs)
                .ThenInclude(s => s.Artist)
                .Include(s => s.Album)
                .Select(s => new SongUpdateVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    ImageUrl = s.ImageUrl,
                    Path = s.Path,
                    Color = s.Color,
                    CreateDate = s.CreateDate,
                    CategoryId = s.CategoryId,
                    AlbumId = s.AlbumId,
                    ArtistsIds = s.ArtistSongs.Select(a => a.ArtistId).ToList(),
                    Category = s.Category,
                    Album = s.Album,
                    ArtistSongs = s.ArtistSongs.ToList()
                }).FirstOrDefault(s => s.Id == id)!;
            return model;
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Song? song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null) NotFound();
            else
            {
                _context.Remove(song);
                await _context.SaveChangesAsync();
                string image = Path.Combine(_env.WebRootPath, "assets/images", song.ImageUrl);

                if (System.IO.File.Exists(image))
                {
                    System.IO.File.Delete(image);
                }
                string fullpath = Path.Combine(_env.WebRootPath, "assets/music", song.Path);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }

            };
            return RedirectToAction("Index");
        }
    }

}
