using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Phases;

public partial class Create
{
    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter]
    public int DevelopmentId { get; set; }

    [SupplyParameterFromForm]
    protected Phase phase { get; set; } = new();

    protected Development? development;
    protected string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        development = await DbContext.Developments.FindAsync(DevelopmentId);

        if (development == null)
        {
            Navigation.NavigateTo("/developments");
            return;
        }

        phase.DevelopmentId = DevelopmentId;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Phases
                .AnyAsync(p =>
                    p.DevelopmentId == DevelopmentId &&
                    p.Name == phase.Name);

            if (exists)
            {
                errorMessage =
                    "Există deja o fază cu acest nume în acest development.";
                return;
            }

            DbContext.Phases.Add(phase);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/phases/{DevelopmentId}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}