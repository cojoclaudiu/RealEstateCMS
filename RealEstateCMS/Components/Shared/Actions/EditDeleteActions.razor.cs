using Microsoft.AspNetCore.Components;

namespace RealEstateCMS.Components.Shared.Actions
{
    public partial class EditDeleteActions
    {
        [Parameter, EditorRequired] 
        public string EditUrl { get; set; } = default!;
        
        [Parameter, EditorRequired] 
        public EventCallback OnDelete { get; set; }
    }
}