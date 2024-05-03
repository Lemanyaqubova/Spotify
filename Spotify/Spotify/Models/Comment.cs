﻿namespace Spotify.Models
{
    public class Comment : BaseEntity
    {
        public string? Message { get; set; }
        public DateTime CreateDate { get; set; }
        public int? SongId { get; set; }
        public Song? Song { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Rating? Rating { get; set; }
    }
}