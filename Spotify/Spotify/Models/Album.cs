namespace Spotify.Models
{
    public class Album : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public List<Song> Songs { get; set; }
        public int? ArtistId { get; set; }
        public Artist Artist { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
