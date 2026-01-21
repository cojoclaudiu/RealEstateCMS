using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.HouseTypes;

public partial class Index
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public int? PhaseId { get; set; }

    protected List<HouseType>? houseTypes;
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

        await LoadHouseTypes();
    }

    protected async Task LoadHouseTypes()
    {
        var query = DbContext.HouseTypes
            .Include(ht => ht.Phase)
                .ThenInclude(p => p.Development)
            .Include(ht => ht.Plots)
            .AsQueryable();

        if (PhaseId.HasValue)
        {
            query = query.Where(ht => ht.PhaseId == PhaseId.Value);
        }

        houseTypes = await query
            .OrderBy(ht => ht.Name)
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
            Navigation.NavigateTo($"/housetypes/{phaseId}");
        }
    }

    protected async Task DeleteHouseType(int houseTypeId)
    {
        try
        {
            var houseType = await DbContext.HouseTypes
                .Include(ht => ht.Plots)
                .FirstOrDefaultAsync(ht => ht.HouseTypeId == houseTypeId);

            if (houseType == null)
                return;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Sigur doriți să ștergeți tipul '{houseType.Name}'?"
            );

            if (!confirmed)
                return;

            if (houseType.Plots.Any())
            {
                await JS.InvokeVoidAsync(
                    "alert",
                    "Nu puteți șterge acest tip deoarece are unități (plots) asociate."
                );
                return;
            }

            DbContext.HouseTypes.Remove(houseType);
            await DbContext.SaveChangesAsync();

            await LoadHouseTypes();
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
