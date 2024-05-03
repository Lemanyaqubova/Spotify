using Spotify.Models;

namespace Spotify.ViewModels
{
    public class CategoryDetailVM
    {
        public ICollection<Song> Songs { get; set; }
        public ICollection<Artist> Artists { get; set; }
        public Artist Artist { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Category> Categories { get; set; }
        public Category Category { get; set; }

    }
}
