using Spotify.Models;

namespace Spotify.ViewModels
{
    public class SongDetailVM
    {
        public List<Song> Songs { get; set; }
        public Song Song { get; set; }
        public Category Category { get; set; }
        public List<Artist> Artists { get; set; }
        public Artist Artist { get; set; }
        public List<ArtistSong> ArtistSongs { get; set; }
        public List<Playlist> Playlists { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }

    }
}
