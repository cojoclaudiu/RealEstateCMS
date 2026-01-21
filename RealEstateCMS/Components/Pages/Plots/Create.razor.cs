using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Enums;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Plots;

public partial class Create
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public int BuildingId { get; set; }

    [SupplyParameterFromForm]
    protected Plot plot { get; set; } = new();

    protected Building? building;
    protected List<HouseType> houseTypes = new();
    protected string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        building = await DbContext.Buildings
            .Include(b => b.Phase)
                .ThenInclude(p => p.Development)
            .FirstOrDefaultAsync(b => b.BuildingId == BuildingId);

        if (building == null)
        {
            Navigation.NavigateTo("/plots");
            return;
        }

        houseTypes = await DbContext.HouseTypes
            .Where(ht =>
                ht.PhaseId == building.PhaseId &&
                ht.IsAvailable)
            .OrderBy(ht => ht.Name)
            .ToListAsync();

        if (!houseTypes.Any())
        {
            errorMessage =
                "Nu există tipuri de locuințe disponibile în această fază. " +
                "Adăugați mai întâi un tip de locuință.";
        }

        if (plot.BuildingId == 0)
        {
            plot.BuildingId = BuildingId;
            plot.Status = PlotStatus.Available;
        }
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Plots
                .AnyAsync(p =>
                    p.BuildingId == BuildingId &&
                    p.Number == plot.Number);

            if (exists)
            {
                errorMessage =
                    "Există deja o unitate cu acest număr în această clădire.";
                return;
            }

            DbContext.Plots.Add(plot);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/plots/{BuildingId}");
        }
        catch (NavigationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}
