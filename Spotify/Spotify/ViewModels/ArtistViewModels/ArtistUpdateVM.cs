﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Spotify.Models;
using System.ComponentModel.DataAnnotations;

namespace Spotify.ViewModels.ArtistViewModels
{
    public class ArtistUpdateVM
    {
        [ValidateNever]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string FullName { get; set; }
        [ValidateNever]
        public IFormFile? Photo { get; set; }
        [ValidateNever]
        public IFormFile? AboutPhoto { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [ValidateNever]
        public string? AboutImg { get; set; }
        [ValidateNever]
        public ICollection<Position> Positions { get; set; } = new List<Position>();
        public List<int> PositionIds { get; set; } = new List<int>();
        public List<ArtistPosition> ArtistPositions { get; set; } = new List<ArtistPosition>();
    }
}
