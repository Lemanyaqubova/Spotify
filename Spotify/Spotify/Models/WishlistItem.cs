namespace Spotify.Models
{
    public class WishlistItem : BaseEntity
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
        public int SongId { get; set; }
        public Song Song { get; set; }
    }
}
