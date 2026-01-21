using Microsoft.JSInterop;

namespace RealEstateCMS.Services;

public class UiDialogService
{
    private readonly IJSRuntime _js;

    public UiDialogService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> Confirm(string message)
        => await _js.InvokeAsync<bool>("confirm", message);

    public async Task Alert(string message)
        => await _js.InvokeVoidAsync("alert", message);
}