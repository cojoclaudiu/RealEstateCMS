using System.ComponentModel.DataAnnotations;
using RealEstateCMS.Data.Enums;

namespace RealEstateCMS.Data.Models
{
    public class HouseType
    {
        public int HouseTypeId { get; set; }

        [Required(ErrorMessage = "Faza este obligatorie")]
        public int PhaseId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 caractere")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numărul de camere este obligatoriu")]
        [Range(0, 10, ErrorMessage = "Numărul de camere trebuie să fie între 0 și 10")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Numărul de băi este obligatoriu")]
        [Range(1, 10, ErrorMessage = "Numărul de băi trebuie să fie între 1 și 10")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Prețul de pornire este obligatoriu")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Prețul trebuie să fie mai mare decât 0")]
        [DataType(DataType.Currency)]
        public decimal FromPrice { get; set; }

        [Required(ErrorMessage = "Tipul proprietății este obligatoriu")]
        public PropertyType PropertyType { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        // Navigation properties
        public Phase Phase { get; set; } = null!;
        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}