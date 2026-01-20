using System.ComponentModel.DataAnnotations;
using RealEstateCMS.Data.Enums;

namespace RealEstateCMS.Data.Models
{
    public class Plot
    {
        public int PlotId { get; set; }

        [Required(ErrorMessage = "Clădirea este obligatorie")]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Tipul de locuință este obligatoriu")]
        public int HouseTypeId { get; set; }

        [Required(ErrorMessage = "Numărul este obligatoriu")]
        [Range(1, int.MaxValue, ErrorMessage = "Numărul trebuie să fie mai mare decât 0")]
        public int Number { get; set; }

        [StringLength(100, ErrorMessage = "Numele nu poate depăși 100 caractere")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Prețul este obligatoriu")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Prețul trebuie să fie mai mare decât 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Status-ul este obligatoriu")]
        public PlotStatus Status { get; set; } = PlotStatus.Available;

        [Range(0, 100, ErrorMessage = "Nivelul trebuie să fie între 0 și 100")]
        public int? Level { get; set; }

        [StringLength(500, ErrorMessage = "Mesajul de marketing nu poate depăși 500 caractere")]
        public string? MarketingMessage { get; set; }

        public bool IsShowHome { get; set; } = false;

        public bool IsFeatured { get; set; } = false;

        // Navigation properties
        public Building Building { get; set; } = null!;
        public HouseType HouseType { get; set; } = null!;
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}