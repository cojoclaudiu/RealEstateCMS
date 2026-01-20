using RealEstateCMS.Components.Shared.Edit;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Developments;

public partial class EditDevelopment : BaseEditPage<Development>
{
    protected Development? development => Entity;

    protected override string NotFoundRedirectUrl => "/developments";

    protected override Task<Development?> LoadEntityAsync(int id)
        => DbContext.Developments.FindAsync(id).AsTask();

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.Developments
                .AnyAsync(d =>
                    d.Name == development!.Name &&
                    d.DevelopmentId != Id);

            if (exists)
            {
                errorMessage = "Există deja un development cu acest nume.";
                return;
            }

            DbContext.Developments.Attach(development!);
            DbContext.Entry(development!).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo("/developments");
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}