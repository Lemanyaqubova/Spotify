using Spotify.Models;
using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.AlbumViewModels
{
    public class AlbumCreateVM
    {
        [Required(ErrorMessage = "Should be not empty")]
        public IFormFile Photo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
