using Microsoft.AspNetCore.Components;
using RealEstateCMS.Data.Models;


namespace RealEstateCMS.Components.Shared.Edit;

public partial class ImageGallery : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<Image> Images { get; set; } = Enumerable.Empty<Image>();

    [Parameter, EditorRequired]
    public string AddImageUrl { get; set; } = string.Empty;
}