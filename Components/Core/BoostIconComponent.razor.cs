namespace FarmSim.Blazor.Components.Core;
public partial class BoostIconComponent
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = ""; // base image key
    [Parameter]
    public TimeSpan? ReduceBy { get; set; } // shows fast-clock badge if set
    [Parameter]
    public bool IsTree { get; set; }
    [Parameter]
    public bool IsAugmented { get; set; }
    [Parameter]
    public int ImageSize { get; set; } = 64;
    [Parameter]
    public int IconSize { get; set; } = 22;
    private string ImageUrl
    {
        get
        {
            if (IsTree)
            {
                return "tree";
            }
            return Name;
        }
    }
}