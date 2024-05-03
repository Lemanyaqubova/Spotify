using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Models;
using Spotify.ViewModels.PositionViewModels;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    //[Authorize(Roles = "Moderator, Admin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PositionController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _context = appDbContext;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Position> model = _context.Positions.AsEnumerable();
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            Position position = _context.Positions
        .Include(p => p.ArtistPositions)
            .ThenInclude(ap => ap.Artist)
        .FirstOrDefault(p => p.Id == id);

            if (position == null)

                return NotFound();

            List<string> artistFullNames = new List<string>();
            foreach (var artistPosition in position.ArtistPositions)
            {
                artistFullNames.Add(artistPosition.Artist.FullName);
            }


            PositionDetailVM viewModel = new PositionDetailVM
            {
                PositionName = position.Name,
                ArtistFullNames = artistFullNames
            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PositionCreateVM position)
        {
            if (!ModelState.IsValid) return View();

            Position newPosition = new()
            {
                Name = position.Name
            };

            await _context.Positions.AddAsync(newPosition);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == 0) return BadRequest();

            PositionUpdateVM position = _context.Positions.Select(p => new PositionUpdateVM()
            {
                Id = p.Id,
                Name = p.Name,

            }).FirstOrDefault(p => p.Id == id)!;
            if (position is null) return NotFound();

            return View(position);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, PositionUpdateVM positionUpdateVM)
        {
            if (id == 0) return BadRequest();

            var position = await _context.Positions.FindAsync(id);

            if (position == null) return NotFound();

            if (!ModelState.IsValid) return View(positionUpdateVM);

            position.Name = positionUpdateVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Position? position = await _context.Positions.FirstOrDefaultAsync(s => s.Id == id);
            if (position == null) NotFound();
            else
            {
                _context.Remove(position);
                _context.SaveChanges();

            };
            return RedirectToAction("Index");
        }

    }
}
