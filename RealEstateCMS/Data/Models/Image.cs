using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstateCMS.Data.Enums;

namespace RealEstateCMS.Data.Models;

public class Image
{
    public int ImageId { get; set; }

    [Required(ErrorMessage = "Tipul proprietarului este obligatoriu")]
    public OwnerType OwnerType { get; set; }

    [Required(ErrorMessage = "ID-ul proprietarului este obligatoriu")]
    [Range(1, int.MaxValue, ErrorMessage = "ID-ul proprietarului trebuie să fie valid")]
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "Numele fișierului este obligatoriu")]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Calea fișierului este obligatorie")]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Helper (not mapped)
    [NotMapped]
    public string ImageUrl => $"/uploads/{FilePath}";
}