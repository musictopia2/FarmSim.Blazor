namespace FarmSim.Blazor.Components.AdvancedSettings;

public partial class AnimalDefaultOptionModeSelector
{
    [Parameter, EditorRequired]
    public EnumAnimalDefaultOption Value { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<EnumAnimalDefaultOption> ValueChanged { get; set; }

    /// <summary>
    /// Optional hook: run persistence + “apply to farm” logic after selection.
    /// </summary>
    [Parameter]
    public EventCallback<EnumAnimalDefaultOption> OnSelected { get; set; }

    private readonly (EnumAnimalDefaultOption Value, string Title, string Description)[] _options =
    [
        (EnumAnimalDefaultOption.Manual,
            "Manual",
            "You have to manually choose the option for animal."),

        (EnumAnimalDefaultOption.Fastest,
            "Fastest",
            "Always choose the fastest option"),

        (EnumAnimalDefaultOption.Slowest,
            "Slowest",
            "Always choose the slowest option.")
    ];

    private async Task SelectAsync(EnumAnimalDefaultOption value)
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