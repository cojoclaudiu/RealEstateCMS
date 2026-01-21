using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;
using RealEstateCMS.Services;
using RealEstateCMS.Data.Enums;

namespace RealEstateCMS.Components.Pages.Images;

public partial class Index
{
    [Inject]
    protected ApplicationDbContext DbContext { get; set; } = default!;

    [Inject]
    protected FileUploadService FileUploadService { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    protected List<Image>? images;
    protected OwnerType? selectedOwnerType;

    protected override async Task OnInitializedAsync()
    {
        await LoadImages();
    }

    protected async Task LoadImages()
    {
        var query = DbContext.Images.AsQueryable();

        if (selectedOwnerType.HasValue)
        {
            query = query.Where(i => i.OwnerType == selectedOwnerType.Value);
        }

        images = await query
            .OrderByDescending(i => i.UploadedAt)
            .ToListAsync();
    }

    protected async Task OnOwnerTypeChanged(ChangeEventArgs e)
    {
        if (Enum.TryParse<OwnerType>(e.Value?.ToString(), out var ownerType))
        {
            selectedOwnerType = ownerType;
        }
        else
        {
            selectedOwnerType = null;
        }

        await LoadImages();
    }

    protected string GetEditUrl(Image image)
    {
        return image.OwnerType switch
        {
            OwnerType.Building => $"/buildings/edit/{image.OwnerId}",
            OwnerType.HouseType => $"/housetypes/edit/{image.OwnerId}",
            OwnerType.Plot => $"/plots/edit/{image.OwnerId}",
            _ => "/"
        };
    }

    protected async Task DeleteImage(Image image)
    {
        var confirmed = await JS.InvokeAsync<bool>(
            "confirm",
            $"Sigur doriți să ștergeți imaginea '{image.FileName}'?"
        );

        if (!confirmed) return;

        try
        {
            FileUploadService.DeleteImage(image.FilePath);

            DbContext.Images.Remove(image);
            await DbContext.SaveChangesAsync();

            await LoadImages();
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync(
                "alert",
                $"Eroare la ștergere: {ex.Message}"
            );
        }
    }
}
