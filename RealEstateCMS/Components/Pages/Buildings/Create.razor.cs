using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Buildings;

public partial class Create
{
    [Inject] 
    protected ApplicationDbContext DbContext { get; set; } = default!;

    [Inject] 
    protected NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public int PhaseId { get; set; }

    [SupplyParameterFromForm]
    protected Building building { get; set; } = new()
    {
        FloorsCount = 1
    };

    protected Phase? phase;
    protected string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        phase = await DbContext.Phases
            .Include(p => p.Development)
            .FirstOrDefaultAsync(p => p.PhaseId == PhaseId);

        if (phase == null)
        {
            Navigation.NavigateTo("/buildings");
            return;
        }

        building.PhaseId = PhaseId;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Buildings.AnyAsync(b =>
                b.PhaseId == PhaseId &&
                b.Name == building.Name);

            if (exists)
            {
                errorMessage = "Există deja o clădire cu acest nume în această fază.";
                return;
            }

            DbContext.Buildings.Add(building);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/buildings/edit/{building.BuildingId}");
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