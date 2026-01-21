using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Plots;

public partial class Index
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public int? BuildingId { get; set; }

    protected List<Plot>? plots;
    protected List<Development> developments = new();
    protected List<Phase> phases = new();
    protected List<Building> buildings = new();

    protected Building? selectedBuilding;
    protected int? selectedDevelopmentId;
    protected int? selectedPhaseId;

    protected override async Task OnInitializedAsync()
    {
        developments = await DbContext.Developments
            .OrderBy(d => d.Name)
            .ToListAsync();

        if (BuildingId.HasValue)
        {
            selectedBuilding = await DbContext.Buildings
                .Include(b => b.Phase)
                    .ThenInclude(p => p.Development)
                .FirstOrDefaultAsync(b => b.BuildingId == BuildingId);
        }

        await LoadPlots();
    }

    protected async Task LoadPlots()
    {
        var query = DbContext.Plots
            .Include(p => p.Building)
            .Include(p => p.HouseType)
            .Include(p => p.Images)
            .AsQueryable();

        if (BuildingId.HasValue)
        {
            query = query.Where(p => p.BuildingId == BuildingId);
        }

        plots = await query
            .OrderBy(p => p.Number)
            .ToListAsync();
    }

    protected async Task OnDevelopmentChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var devId))
        {
            selectedDevelopmentId = devId;

            phases = await DbContext.Phases
                .Where(p => p.DevelopmentId == devId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            buildings.Clear();
            selectedPhaseId = null;
        }
        else
        {
            selectedDevelopmentId = null;
            phases.Clear();
            buildings.Clear();
        }
    }

    protected async Task OnPhaseChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var phaseId))
        {
            selectedPhaseId = phaseId;

            buildings = await DbContext.Buildings
                .Where(b => b.PhaseId == phaseId)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }
        else
        {
            selectedPhaseId = null;
            buildings.Clear();
        }
    }

    protected void OnBuildingChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var buildingId))
        {
            Navigation.NavigateTo($"/plots/{buildingId}");
        }
    }

    protected async Task DeletePlot(int plotId)
    {
        try
        {
            var plot = await DbContext.Plots
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.PlotId == plotId);

            if (plot == null)
                return;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Sigur doriți să ștergeți unitatea '{plot.Name ?? plot.Number.ToString()}'?"
            );

            if (!confirmed)
                return;

            DbContext.Plots.Remove(plot);
            await DbContext.SaveChangesAsync();

            await LoadPlots();
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
