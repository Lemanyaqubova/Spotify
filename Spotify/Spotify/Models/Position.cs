namespace Spotify.Models
{
    public class Position : BaseEntity
    {
        public string Name { get; set; }
        public List<ArtistPosition> ArtistPositions { get; set; }
    }
}
