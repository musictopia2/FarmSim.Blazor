namespace FarmSim.Blazor.Components.Trees;
public partial class TreeImageComponent
{
    [Parameter, EditorRequired]
    public string ItemName { get; set; } = ""; //needs to know the item being produced (so shows the proper image).
    [Parameter, EditorRequired]
    public int DisplayAmount { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public EventCallback OnClick { get; set; }
}