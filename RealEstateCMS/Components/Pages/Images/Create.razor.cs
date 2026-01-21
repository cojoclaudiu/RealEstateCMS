using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Enums;
using RealEstateCMS.Data.Models;
using RealEstateCMS.Services;

namespace RealEstateCMS.Components.Pages.Images;

public partial class Create
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected FileUploadService FileUploadService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter] public string OwnerTypeName { get; set; } = string.Empty;
    [Parameter] public int OwnerId { get; set; }

    protected Image image = new();
    protected IBrowserFile? selectedFile;
    protected string? previewUrl;
    protected string? fileError;
    protected string? errorMessage;
    protected bool isUploading;

    protected override void OnInitialized()
    {
        if (!Enum.TryParse<OwnerType>(
                OwnerTypeName,
                true,
                out var parsedOwnerType))
        {
            Navigation.NavigateTo("/");
            return;
        }

        image.OwnerType = parsedOwnerType;
        image.OwnerId = OwnerId;
    }

    protected async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        fileError = null;
        selectedFile = e.File;

        if (selectedFile == null)
            return;

        image.FileName = selectedFile.Name;
        image.FilePath = "temp"; // needed for validation

        await CreatePreview();
    }

    protected async Task CreatePreview()
    {
        try
        {
            var resized = await selectedFile!.RequestImageFileAsync(
                selectedFile.ContentType, 800, 800);

            using var stream = resized.OpenReadStream(5 * 1024 * 1024);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            previewUrl =
                $"data:{selectedFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        }
        catch
        {
            previewUrl = null;
        }
    }

    protected async Task HandleValidSubmit()
    {
        if (selectedFile == null)
        {
            errorMessage = "Selectați o imagine.";
            return;
        }

        isUploading = true;
        errorMessage = null;

        try
        {
            var (success, filePath, uploadError) =
                await FileUploadService.UploadImageAsync(
                    selectedFile,
                    image.OwnerType.ToString().ToLower());

            if (!success)
            {
                errorMessage = uploadError;
                return;
            }

            image.FilePath = filePath!;
            image.UploadedAt = DateTime.Now;

            if (image.IsPrimary)
            {
                var existing = await DbContext.Images
                    .Where(i =>
                        i.OwnerType == image.OwnerType &&
                        i.OwnerId == image.OwnerId &&
                        i.IsPrimary)
                    .ToListAsync();

                foreach (var img in existing)
                    img.IsPrimary = false;
            }

            DbContext.Images.Add(image);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo(GetRedirectUrl());
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(image.FilePath))
                FileUploadService.DeleteImage(image.FilePath);

            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
        finally
        {
            isUploading = false;
        }
    }

    protected void Cancel()
        => Navigation.NavigateTo(GetRedirectUrl());

    protected string GetRedirectUrl() => image.OwnerType switch
    {
        OwnerType.Building => $"/buildings/edit/{OwnerId}",
        OwnerType.HouseType => $"/housetypes/edit/{OwnerId}",
        OwnerType.Plot => $"/plots/edit/{OwnerId}",
        _ => "/"
    };
}
