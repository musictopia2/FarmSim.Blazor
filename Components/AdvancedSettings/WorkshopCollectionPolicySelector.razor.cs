namespace FarmSim.Blazor.Components.AdvancedSettings;

public partial class WorkshopCollectionPolicySelector
{
    [Parameter, EditorRequired]
    public EnumWorkshopCollectionCategories Value { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<EnumWorkshopCollectionCategories> ValueChanged { get; set; }

    /// <summary>Optional hook for “apply to farm now” mutations.</summary>
    [Parameter]
    public EventCallback<EnumWorkshopCollectionCategories> OnSelected { get; set; }

    private readonly (EnumWorkshopCollectionCategories Value, string Title, string Description)[] _options =
    [
        (EnumWorkshopCollectionCategories.FullyManual,
            "Fully manual",
            "You collect one at a time from a workshop."),

        (EnumWorkshopCollectionCategories.AutomateSingleInstance,
            "Automate one at a time",
            "Automatically collect a single finished workshop."),

        (EnumWorkshopCollectionCategories.FullyAutomated,
            "Fully automated",
            "Automatically collect all completed workshops.")
    ];

    private async Task SelectAsync(EnumWorkshopCollectionCategories value)
    {
        if (Value == value)
        {
            return;
        }

        // Update UI state immediately
        await ValueChanged.InvokeAsync(value);

        // Then run side-effects (persist, mutate farms, etc.)
        if (OnSelected.HasDelegate)
        {
            await OnSelected.InvokeAsync(value);
        }
    }
}