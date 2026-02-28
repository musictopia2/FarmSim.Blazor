namespace FarmSim.Blazor.Components.AdvancedSettings;

public partial class AnimalCollectionModeSelector
{
    [Parameter, EditorRequired]
    public EnumAnimalCollectionMode Value { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<EnumAnimalCollectionMode> ValueChanged { get; set; }

    /// <summary>
    /// Optional hook: run persistence + “apply to farm” logic after selection.
    /// </summary>
    [Parameter]
    public EventCallback<EnumAnimalCollectionMode> OnSelected { get; set; }

    private readonly (EnumAnimalCollectionMode Value, string Title, string Description)[] _options =
    [
        (EnumAnimalCollectionMode.OneAtTime,
            "One at a time",
            "You collect one at at time for an animal."),

        (EnumAnimalCollectionMode.AllAtOnce,
            "All at once",
            "You collect all at once from an animal"),

        (EnumAnimalCollectionMode.Automated,
            "Automated",
            "Automatically collects from animals.")
    ];

    private async Task SelectAsync(EnumAnimalCollectionMode value)
    {
        if (Value == value)
        {
            return;
        }

        // Update bound value first (instant UI)
        await ValueChanged.InvokeAsync(value);

        // Then side effects (save + mutate farms)
        if (OnSelected.HasDelegate)
        {
            await OnSelected.InvokeAsync(value);
        }
    }
}