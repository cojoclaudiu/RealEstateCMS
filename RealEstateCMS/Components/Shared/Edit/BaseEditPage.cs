using Microsoft.AspNetCore.Components;
using RealEstateCMS.Data;

namespace RealEstateCMS.Components.Shared.Edit;

/// <summary>
/// Base class for Edit pages.
/// Contains shared infrastructure (Id, DbContext, Navigation, error handling).
/// </summary>
public abstract class BaseEditPage<TEntity> : ComponentBase
    where TEntity : class
{
    [Parameter] public int Id { get; set; }

    [Inject] protected ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected TEntity? Entity;
    protected string? errorMessage;

    /// <summary>
    /// Must load the entity with all required Includes.
    /// Return null if not found.
    /// </summary>
    protected abstract Task<TEntity?> LoadEntityAsync(int id);

    /// <summary>
    /// Where to redirect if entity is not found.
    /// </summary>
    protected abstract string NotFoundRedirectUrl { get; }

    protected override async Task OnInitializedAsync()
    {
        Entity = await LoadEntityAsync(Id);

        if (Entity == null)
        {
            Navigation.NavigateTo(NotFoundRedirectUrl);
        }
    }
}