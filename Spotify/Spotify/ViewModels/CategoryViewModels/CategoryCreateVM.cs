using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.CategoryViewModels
{
    public class CategoryCreateVM
    {
        [Required]
        [StringLength(maximumLength: 25, MinimumLength = 3)]
        public string Name { get; set; }
        public string Color { get; set; }
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
