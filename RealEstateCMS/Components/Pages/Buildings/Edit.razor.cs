using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Components.Shared.Edit;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Buildings;

public partial class EditBuilding : BaseEditPage<Building>
{
    protected Building? building => Entity;

    protected override string NotFoundRedirectUrl => "/buildings";

    protected override async Task<Building?> LoadEntityAsync(int id)
    {
        var building = await DbContext.Buildings
            .Include(b => b.Phase)
            .ThenInclude(p => p.Development)
            .Include(b => b.Plots)
            .FirstOrDefaultAsync(b => b.BuildingId == id);

        if (building == null)
            return null;

        building.Images = await DbContext.Images
            .Where(i =>
                i.OwnerType == Data.Enums.OwnerType.Building &&
                i.OwnerId == building.BuildingId)
            .OrderByDescending(i => i.IsPrimary)
            .ThenByDescending(i => i.UploadedAt)
            .ToListAsync();

        return building;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Buildings
                .AnyAsync(b =>
                    b.PhaseId == building!.PhaseId &&
                    b.Name == building.Name &&
                    b.BuildingId != Id);

            if (exists)
            {
                errorMessage = "Există deja o clădire cu acest nume în această fază.";
                return;
            }

            var buildingToUpdate = await DbContext.Buildings
                .Include(b => b.Plots)
                .FirstOrDefaultAsync(b => b.BuildingId == Id);

            if (buildingToUpdate == null)
            {
                errorMessage = "Clădirea nu a fost găsită.";
                return;
            }

            // Optional safety rule
            if (buildingToUpdate.Plots.Any(p => p.Level > building!.FloorsCount))
            {
                errorMessage = "Numărul de etaje este mai mic decât etajele existente.";
                return;
            }

            buildingToUpdate.Name = building!.Name;
            buildingToUpdate.FloorsCount = building.FloorsCount;

            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/buildings/{building!.PhaseId}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}
