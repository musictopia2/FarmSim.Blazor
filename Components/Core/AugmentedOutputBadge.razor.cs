namespace FarmSim.Blazor.Components.Core;
public partial class AugmentedOutputBadge
{
    // lets your parent pass class="boost-icon-badge" etc.
    [Parameter] public string CssClass { get; set; } = "";

    // optional sizing if you ever use it standalone
    [Parameter] public int Size { get; set; } = 22;
}