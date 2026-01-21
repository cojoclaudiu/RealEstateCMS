using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Data;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.Developments;

public partial class Create
{
    [Inject]
    protected ApplicationDbContext DbContext { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm]
    protected Development development { get; set; } = new();

    protected string? errorMessage;

    protected async Task HandleValidSubmit()
    {
        try
        {
            // Normalize input
            development.Name = development.Name?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(development.Location))
            {
                development.Location = development.Location.Trim();
            }

            var exists = await DbContext.Developments
                .AnyAsync(d => d.Name == development.Name);

            if (exists)
            {
                errorMessage = "Există deja un development cu acest nume.";
                return;
            }

            development.CreatedAt = DateTime.Now;

            DbContext.Developments.Add(development);
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo("/developments");
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

    protected void HandleInvalidSubmit()
    {
        errorMessage = "Completează câmpurile obligatorii corect.";
    }
}