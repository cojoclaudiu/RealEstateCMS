using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Developments;

public partial class Index
{
    [Inject]
    protected ApplicationDbContext DbContext { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    protected List<Development>? developments;
    protected string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadDevelopments();
    }

    protected async Task LoadDevelopments()
    {
        developments = await DbContext.Developments
            .Include(d => d.Phases)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    protected async Task DeleteDevelopment(int developmentId)
    {
        try
        {
            var development = await DbContext.Developments
                .Include(d => d.Phases)
                .FirstOrDefaultAsync(d => d.DevelopmentId == developmentId);

            if (development == null)
            {
                errorMessage = "Development-ul nu a fost găsit.";
                return;
            }

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                $"Sigur doriți să ștergeți development-ul '{development.Name}'?" +
                (development.Phases.Any()
                    ? " Această acțiune va șterge și toate fazele asociate."
                    : "")
            );

            if (!confirmed)
                return;

            if (development.Phases.Any())
            {
                await JS.InvokeVoidAsync(
                    "alert",
                    "Nu puteți șterge acest development deoarece are faze asociate. Ștergeți mai întâi fazele."
                );
                return;
            }

            DbContext.Developments.Remove(development);
            await DbContext.SaveChangesAsync();

            errorMessage = null;
            await LoadDevelopments();
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la ștergere: {ex.Message}";
        }
    }
}
