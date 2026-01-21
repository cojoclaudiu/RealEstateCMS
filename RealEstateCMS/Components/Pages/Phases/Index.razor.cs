using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Phases;

public partial class Index
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public int? DevelopmentId { get; set; }

    protected List<Phase>? phases;
    protected List<Development> developments = new();
    protected Development? selectedDevelopment;

    protected override async Task OnInitializedAsync()
    {
        developments = await DbContext.Developments
            .OrderBy(d => d.Name)
            .ToListAsync();

        if (DevelopmentId.HasValue)
        {
            selectedDevelopment = await DbContext.Developments
                .FindAsync(DevelopmentId.Value);
        }

        await LoadPhases();
    }

    protected async Task LoadPhases()
    {
        var query = DbContext.Phases
            .Include(p => p.Development)
            .Include(p => p.Buildings)
            .Include(p => p.HouseTypes)
            .AsQueryable();

        if (DevelopmentId.HasValue)
        {
            query = query.Where(p => p.DevelopmentId == DevelopmentId.Value);
        }

        phases = await query
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    protected void OnDevelopmentChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var devId))
        {
            Navigation.NavigateTo($"/phases/{devId}");
        }
        else
        {
            Navigation.NavigateTo("/phases");
        }
    }

    protected async Task DeletePhase(int phaseId)
    {
        try
        {
            var phase = await DbContext.Phases
                .Include(p => p.Buildings)
                .Include(p => p.HouseTypes)
                .FirstOrDefaultAsync(p => p.PhaseId == phaseId);

            if (phase == null)
                return;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Sigur doriți să ștergeți faza '{phase.Name}'?"
            );

            if (!confirmed)
                return;

            if (phase.Buildings.Any() || phase.HouseTypes.Any())
            {
                await JS.InvokeVoidAsync(
                    "alert",
                    "Nu puteți șterge această fază deoarece are clădiri sau tipuri de locuințe asociate."
                );
                return;
            }

            DbContext.Phases.Remove(phase);
            await DbContext.SaveChangesAsync();

            await LoadPhases();
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
