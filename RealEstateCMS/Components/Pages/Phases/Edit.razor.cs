using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Components.Shared.Edit;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Phases;

public partial class EditPhase : BaseEditPage<Phase>
{
    // mapăm Entity → phase pentru UI
    protected Phase? phase => Entity;

    protected override string NotFoundRedirectUrl => "/phases";

    protected override async Task<Phase?> LoadEntityAsync(int id)
    {
        return await DbContext.Phases
            .Include(p => p.Development)
            .FirstOrDefaultAsync(p => p.PhaseId == id);
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Phases
                .AnyAsync(p =>
                    p.DevelopmentId == phase!.DevelopmentId &&
                    p.Name == phase.Name &&
                    p.PhaseId != Id);

            if (exists)
            {
                errorMessage = "Există deja o fază cu acest nume în acest development.";
                return;
            }

            DbContext.Phases.Attach(phase!);
            DbContext.Entry(phase!).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/phases/{phase!.DevelopmentId}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}