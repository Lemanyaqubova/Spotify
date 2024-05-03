using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.Extensions;
using Spotify.Models;
using Spotify.ViewModels.CategoryViewModels;

namespace Spotify.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    //[Authorize(Roles = "Moderator, Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _context = appDbContext;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> model = _context.Categories.AsEnumerable();
            return View(model);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();

            Category category = await _context.Categories.Include(c => c.Albums).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM categoryCreateVM)
        {

            if (categoryCreateVM.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please Input Image");
                return View();

            }
            if (!categoryCreateVM.Photo.IsImage())
            {
                ModelState.AddModelError("photo", "please input only image");
                return View();
            }
            Category newCategory = new()
            {
                Name = categoryCreateVM.Name,
                ImageUrl = categoryCreateVM.Photo.SaveImage(_env, "assets/images", categoryCreateVM.Photo.FileName),

                Color = categoryCreateVM.Color
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return NotFound();
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(new CategoryUpdateVM
            {
                ImageUrl = category.ImageUrl,
                Name = category.Name,
                Color = category.Color
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM categoryUpdateVM)
        {
            if (id == null) return NotFound();
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            if (categoryUpdateVM.Photo != null)
            {
                if (categoryUpdateVM.Photo == null)
                {
                    ModelState.AddModelError("Photo", "bosh qoyma");
                    return View(categoryUpdateVM = new() { ImageUrl = category.ImageUrl });
                }
                if (!categoryUpdateVM.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "only image select");
                    return View(categoryUpdateVM = new() { ImageUrl = category.ImageUrl });
                }
                string fullPath = Path.Combine(_env.WebRootPath, "assets/images", category.ImageUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                category.ImageUrl = categoryUpdateVM.Photo.SaveImage(_env, "assets/images", categoryUpdateVM.Photo.FileName);
                _context.SaveChanges();
            }
            category.Name = categoryUpdateVM.Name;
            category.Color = categoryUpdateVM.Color;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Category? category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (category == null) NotFound();



            else
            {
                _context.Remove(category);
                _context.SaveChanges();
                string fullpath = Path.Combine(_env.WebRootPath, "assets/images", category.ImageUrl);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
            };
            return RedirectToAction("Index");
        }
    }
}
