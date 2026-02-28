namespace FarmSim.Blazor.Components.Store;
public partial class AcquireConfirmation
{
    [Parameter]
    [EditorRequired]
    public StoreItemRowModel StoreItem { get; set; }
    [Parameter]
    public EventCallback OnAcquire { get; set; } //you may already know because you have to set the parameter anyways.
    [Parameter]
    public EventCallback OnClose { get; set; }
    private string ImageUrl
    {
        get
        {
            if (StoreItem.Category == EnumCatalogCategory.Tree)
            {
                return "tree";
            }
            return StoreItem.TargetName;
        }
    }
}