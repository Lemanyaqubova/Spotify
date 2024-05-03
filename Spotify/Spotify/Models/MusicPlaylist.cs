namespace Spotify.Models
{
    public class MusicPlaylist : BaseEntity
    {
        public int SongId { get; set; }
        public Song Song { get; set; }
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
    }
}
