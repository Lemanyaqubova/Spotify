using Spotify.Models;

namespace Spotify.ViewModels.AlbumViewModels
{
    public class AlbumUpdateVM
    {

        public IFormFile? Photo { get; set; }

        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
        public int? ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
