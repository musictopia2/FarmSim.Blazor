namespace FarmSim.Blazor.Components.Core;
public partial class ItemRequirementDisplay
{


    [Parameter]
    public EventCallback<string> OnClicked { get; set; }
    [Parameter]
    public bool OnWhiteBackground { get; set; } = true;

    [Parameter]
    public bool ShowName { get; set; } = true;
    [Parameter] public bool ShowBorder { get; set; } = false;

    [Parameter]
    public string Name { get; set; } = "";
    [Parameter]
    public int Required { get; set; }
    //for now, would be from inventory (if i have in barn, rethink).
    [Parameter]
    public int Have { get; set; }
    private bool IsClickable => OnClicked.HasDelegate;


    [Parameter] public string? Alt { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    [Parameter] public string? ImageClass { get; set; }
    [Parameter] public string? AmountClass { get; set; }
    [Parameter] public string? NameStyle { get; set; }
    [Parameter] public int? WidthPx { get; set; }

    private void Process()
    {
        if (OnClicked.HasDelegate)
        {
            OnClicked.InvokeAsync(Name);
        }
    }
    private string? ComputedStyle =>
        $"{(WidthPx.HasValue ? $"width:{WidthPx.Value}px;" : "")}{Style}".Trim();

    [Parameter] public EnumRequirementVariant Variant { get; set; } = EnumRequirementVariant.Row;
    private bool IsComplete => Have >= Required;
    private string AltText => string.IsNullOrWhiteSpace(Alt) ? Name : Alt!;
    private string RootClass =>
        $"ird-root {(Variant == EnumRequirementVariant.Row ? "ird-row" : "ird-tile")} " +
        $"{(ShowBorder ? "ird-bordered" : "ird-borderless")} " +
        $"{Class}"
        .Trim();

    private string ImgClass => $"{(Variant == EnumRequirementVariant.Row ? "ird-img-sm" : "ird-img-lg")} {ImageClass}".Trim();
    private string AmountCss
    {
        get
        {
            string firsts = "ird-amount ";
            string? lasts = AmountClass;

            string middles;
            if (IsComplete && OnWhiteBackground)
            {
                middles = "ird-ok";
            }
            else if (IsComplete)
            {
                middles = "black-ok";
            }
            else if (OnWhiteBackground)
            {
                middles = "ird-bad";
            }
            else
            {
                middles = "black-bad";
            }
            if (lasts is null)
            {
                return $"{firsts} {middles}";
            }
            return $"{firsts} {middles} {lasts}";
        }
    }


}