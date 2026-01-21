using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Buildings;

public partial class Index
{
    [Inject]
    protected ApplicationDbContext DbContext { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public int? PhaseId { get; set; }

    protected List<Building>? buildings;
    protected List<Development> developments = new();
    protected List<Phase> phases = new();
    protected Phase? selectedPhase;
    protected int? selectedDevelopmentId;

    protected override async Task OnInitializedAsync()
    {
        developments = await DbContext.Developments
            .OrderBy(d => d.Name)
            .ToListAsync();

        if (PhaseId.HasValue)
        {
            selectedPhase = await DbContext.Phases
                .Include(p => p.Development)
                .FirstOrDefaultAsync(p => p.PhaseId == PhaseId.Value);
        }

        await LoadBuildings();
    }

    protected async Task LoadBuildings()
    {
        var baseQuery = DbContext.Buildings
            .Include(b => b.Phase)
                .ThenInclude(p => p.Development)
            .Include(b => b.Plots)
            .Where(b => !PhaseId.HasValue || b.PhaseId == PhaseId.Value);

        buildings = await baseQuery
            .OrderBy(b => b.Name)
            .ToListAsync();

        var imageCounts = await DbContext.Images
            .Where(i => i.OwnerType == Data.Enums.OwnerType.Building)
            .GroupBy(i => i.OwnerId)
            .Select(g => new { BuildingId = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var building in buildings)
        {
            var count = imageCounts
                .FirstOrDefault(x => x.BuildingId == building.BuildingId);

            building.Images = count == null
                ? new List<Image>()
                : Enumerable.Repeat(new Image(), count.Count).ToList();
        }
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
        }
        else
        {
            selectedDevelopmentId = null;
            phases.Clear();
        }
    }

    protected void OnPhaseChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var phaseId))
        {
            Navigation.NavigateTo($"/buildings/{phaseId}");
        }
    }

    protected async Task DeleteBuilding(Building building)
    {
        var confirmed = await JS.InvokeAsync<bool>(
            "confirm",
            $"Sigur doriți să ștergeți clădirea '{building.Name}'?"
        );

        if (!confirmed) return;

        if (building.Plots.Any())
        {
            await JS.InvokeVoidAsync(
                "alert",
                "Nu puteți șterge această clădire deoarece are unități asociate."
            );
            return;
        }

        DbContext.Buildings.Remove(building);
        await DbContext.SaveChangesAsync();
        await LoadBuildings();
    }
}
