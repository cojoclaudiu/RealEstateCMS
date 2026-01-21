using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.HouseTypes;

public partial class Create
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public int PhaseId { get; set; }

    protected HouseType houseType = new()
    {
        IsAvailable = true
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
            Navigation.NavigateTo("/housetypes");
            return;
        }

        houseType.PhaseId = PhaseId;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.HouseTypes
                .AnyAsync(ht =>
                    ht.PhaseId == PhaseId &&
                    ht.Name == houseType.Name);

            if (exists)
            {
                errorMessage = "Există deja un tip cu acest nume în această fază.";
                return;
            }

            DbContext.HouseTypes.Add(houseType);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/housetypes/edit/{houseType.HouseTypeId}");
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