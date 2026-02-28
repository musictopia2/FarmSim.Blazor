namespace FarmSim.Blazor.Components.Core;
public partial class BasicProgressModal
{
    [Parameter]
    public bool Visible { get; set; }
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] 
    public RenderFragment? ChildContent { get; set; }
}