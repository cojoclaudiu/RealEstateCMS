using Microsoft.EntityFrameworkCore;
using RealEstateCMS.Components.Shared.Edit;
using RealEstateCMS.Data.Models;

namespace RealEstateCMS.Components.Pages.HouseTypes;

public partial class EditHouseType : BaseEditPage<HouseType>
{
    protected HouseType? houseType => Entity;

    protected override string NotFoundRedirectUrl => "/housetypes";

    protected override async Task<HouseType?> LoadEntityAsync(int id)
    {
        return await DbContext.HouseTypes
            .Include(ht => ht.Phase)
            .ThenInclude(p => p.Development)
            .Include(ht => ht.Images)
            .FirstOrDefaultAsync(ht => ht.HouseTypeId == id);
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            var exists = await DbContext.HouseTypes
                .AnyAsync(ht =>
                    ht.PhaseId == houseType!.PhaseId &&
                    ht.Name == houseType.Name &&
                    ht.HouseTypeId != Id);

            if (exists)
            {
                errorMessage = "Există deja un tip cu acest nume în această fază.";
                return;
            }

            DbContext.HouseTypes.Attach(houseType!);
            DbContext.Entry(houseType!).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();

            Navigation.NavigateTo($"/housetypes/{houseType!.PhaseId}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Eroare la salvare: {ex.Message}";
        }
    }
}