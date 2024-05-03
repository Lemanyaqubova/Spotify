﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.DAL;
using Spotify.ViewModels;

namespace Spotify.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var homeVM = new HomeVM
            {
                Albums = await _context.Albums.Include(m => m.Category).Include(m => m.Artist).OrderByDescending(a => a.Id).Take(8).ToListAsync(),
            };

            return View(homeVM);
        }
    }
}
