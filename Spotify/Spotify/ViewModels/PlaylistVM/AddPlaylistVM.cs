﻿namespace Spotify.ViewModels.PlaylistVM
{
    public class AddPlaylistVM
    {
        public IFormFile Photo { get; set; }
        public string ImgUrl { get; set; }
        public string? Name { get; set; }
        public List<int> Ids { get; set; }
    }
}
