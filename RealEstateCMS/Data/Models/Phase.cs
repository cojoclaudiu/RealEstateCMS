using System.ComponentModel.DataAnnotations;

namespace RealEstateCMS.Data.Models
{
    public class Phase
    {
        public int PhaseId { get; set; }

        [Required(ErrorMessage = "Development-ul este obligatoriu")]
        public int DevelopmentId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 caractere")]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        // Navigation properties
        public Development Development { get; set; } = null!;
        public ICollection<Building> Buildings { get; set; } = new List<Building>();
        public ICollection<HouseType> HouseTypes { get; set; } = new List<HouseType>();
    }
}