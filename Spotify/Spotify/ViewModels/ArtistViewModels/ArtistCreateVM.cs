using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Spotify.Models;
using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.ArtistViewModels
{
    public class ArtistCreateVM
    {
        [Required, MaxLength(50)]
        public string FullName { get; set; }
        public IFormFile Photo { get; set; } = null!;
        public IFormFile AboutImg { get; set; } = null!;

        [ValidateNever]
        public ICollection<Position> Positions { get; set; } = null!;
        [Required]
        public ICollection<int> PositionIds { get; set; } = null!;
    }
}
