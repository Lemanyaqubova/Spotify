﻿using Spotify.Models;

namespace Spotify.ViewModels
{
    public class ArtistDetailVM
    {

        public IEnumerable<Artist> Artists { get; set; }
        public IEnumerable<ArtistSong> ArtistSongs { get; set; }
        public Artist Artist { get; set; }

        public IEnumerable<Album> Albums { get; set; }

        public Rating Rating { get; set; }
        public ICollection<Artist> SamePosition { get; set; }
    }
}
