using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Plots;

public partial class Edit : ComponentBase
{
    [Parameter] public int Id { get; set; }

    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm]
    protected Plot? plot { get; set; }

    protected List<HouseType> houseTypes = new();
    protected string? errorMessage;

    protected int phaseId;
    protected int buildingId;

    protected override async Task OnInitializedAsync()
    {
        var loadedPlot = await DbContext.Plots
            .Include(p => p.Building)
                .ThenInclude(b => b.Phase)
                    .ThenInclude(ph => ph.Development)
            .Include(p => p.HouseType)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.PlotId == Id);

        if (loadedPlot == null)
        {
            Navigation.NavigateTo("/plots");
            return;
        }

        phaseId = loadedPlot.Building.PhaseId;
        buildingId = loadedPlot.BuildingId;

        if (plot == null)
        {
            plot = loadedPlot;
        }
        else
        {
            plot.Building = loadedPlot.Building;
            plot.HouseType = loadedPlot.HouseType;
            plot.Images = loadedPlot.Images;
        }

        houseTypes = await DbContext.HouseTypes
            .Where(ht => ht.PhaseId == phaseId)
            .OrderBy(ht => ht.Name)
            .ToListAsync();
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Plots
                .AnyAsync(p =>
                    p.BuildingId == plot!.BuildingId &&
                    p.Number == plot.Number &&
                    p.PlotId != Id);

            if (exists)
            {
                errorMessage = "Există deja o unitate cu acest număr în această clădire.";
                return;
            }

            DbContext.Entry(plot!).State = EntityState.Detached;

            var plotToUpdate = await DbContext.Plots
                .FirstOrDefaultAsync(p => p.PlotId == Id);

            if (plotToUpdate == null)
            {
                errorMessage = "Unitatea nu a fost găsită.";
                return;
            }

            plotToUpdate.HouseTypeId = plot.HouseTypeId;
            plotToUpdate.Number = plot.Number;
            plotToUpdate.Level = plot.Level;
            plotToUpdate.Name = plot.Name;
            plotToUpdate.Price = plot.Price;
            plotToUpdate.Status = plot.Status;
            plotToUpdate.MarketingMessage = plot.MarketingMessage;
            plotToUpdate.IsShowHome = plot.IsShowHome;
            plotToUpdate.IsFeatured = plot.IsFeatured;

            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/plots/{buildingId}");
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
