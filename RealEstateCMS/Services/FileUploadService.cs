using Microsoft.AspNetCore.Components.Forms;

namespace RealEstateCMS.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadImageAsync(
            IBrowserFile file, 
            string subfolder = "")
        {
            try
            {
                // Validate file size
                if (file.Size > _maxFileSize)
                {
                    return (false, null, "Fișierul este prea mare. Mărimea maximă este 5MB.");
                }

                // Validate extension
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return (false, null, "Tipul fișierului nu este permis. Folosiți: jpg, jpeg, png, gif, webp.");
                }

                // Create unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                
                // Create subfolder path if provided
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!string.IsNullOrEmpty(subfolder))
                {
                    uploadPath = Path.Combine(uploadPath, subfolder);
                }

                // Ensure directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Full file path
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save file
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);

                // Return relative path for database
                var relativePath = string.IsNullOrEmpty(subfolder) 
                    ? uniqueFileName 
                    : Path.Combine(subfolder, uniqueFileName).Replace("\\", "/");

                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Eroare la upload: {ex.Message}");
            }
        }

        public bool DeleteImage(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, "uploads", filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}