using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.SongViewModels
{
    public class SongCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
        public IFormFile Audio { get; set; } = null!;

        [Required]
        //public string Category { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string Color { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public int AlbumId { get; set; }

        //public List<Album> Albums { get; set; } = new List<Album>();
        //[ValidateNever]
        //public ICollection<Artist>? Artists { get; set; } = null!;

        [Required]
        [Display(Prompt = "Artists")]
        public ICollection<int> ArtistIds { get; set; } = null!;
        //public SongCreateVM()
        //{
        //    ArtistIds = new List<int>();
        //}
    }
}
