namespace FarmSim.Blazor.Components.Inventory;

public partial class InventoryAmountIndicator
{
    [Parameter, EditorRequired]
    public string ItemName { get; set; } = "";

    // Optional icon override (defaults to Barn)
    [Parameter]
    public string IconName { get; set; }
        = Images.Standard.Assets.PngImageAssets.Barn;

    

    [Parameter]
    public int? Height { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }
    [Parameter]
    public string FontSize { get; set; } = "1rem";
    [Parameter]
    public bool ShowIcon { get; set; } = true;
    private int CurrentAmount =>
       InventoryManager.Get(ItemName);

    private string CssClass =>
        $"inventory-amount-indicator {Class}".Trim();

    private string FinalStyle =>
        string.IsNullOrWhiteSpace(Style)
            ? DefaultStyle
            : $"{DefaultStyle} {Style}";

    private string DefaultStyle =>
        "display:flex;align-items:center;gap:8px;justify-content:flex-end;";

    private int ResolvedHeight => Height ?? 30;

}