using System.ComponentModel.DataAnnotations;
using RealEstateCMS.Data.Validation;

namespace RealEstateCMS.Data.Models
{
    public class Building
    {
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Faza este obligatorie")]
        public int PhaseId { get; set; }

        [RequiredTrimmed(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Numele trebuie să aibă între 2 și 100 caractere")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Numărul de etaje trebuie să fie între 1 și 100")]
        public int FloorsCount { get; set; }

        // Navigation
        public Phase Phase { get; set; } = null!;
        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}