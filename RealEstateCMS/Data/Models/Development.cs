using System.ComponentModel.DataAnnotations;
using RealEstateCMS.Data.Validation;

namespace RealEstateCMS.Data.Models
{
    public class Development
    {
        public int DevelopmentId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [RequiredTrimmed(ErrorMessage = "Numele nu poate fi doar spații")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Numele trebuie să aibă între 3 și 100 caractere"
        )]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Locația nu poate depăși 200 caractere")]
        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Phase> Phases { get; set; } = new List<Phase>();
    }
}