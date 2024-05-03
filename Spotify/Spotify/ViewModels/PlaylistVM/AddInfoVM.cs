namespace Spotify.ViewModels.PlaylistVM
{
    public class AddInfoVM
    {
        public string PlaylistPhotoUrl { get; set; }
        public IFormFile PlaylistImg { get; set; }
        public string Name { get; set; } = null!;
        public List<int> Ids { get; set; }
    }
}
