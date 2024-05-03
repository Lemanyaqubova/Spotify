namespace Spotify.ViewModels.SongViewModels
{
    public class SongListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public DateTime CreateDate { get; set; }
        public string AlbumName { get; set; }
    }
}
