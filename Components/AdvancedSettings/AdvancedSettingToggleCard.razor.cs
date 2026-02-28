namespace FarmSim.Blazor.Components.AdvancedSettings;

public partial class AdvancedSettingToggleCard
{
    /// <summary>Displayed title.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>Optional short description (1 sentence works best).</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Optional smaller text line for extra clarity (use sparingly).</summary>
    [Parameter] public string? SecondaryText { get; set; }

    /// <summary>Current value.</summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>Two-way bind support.</summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>Called after toggle (useful if you want to run farm mutation code).</summary>
    [Parameter] public EventCallback<bool> OnToggled { get; set; }

    /// <summary>Disable interaction.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Text shown in the pill when ON.</summary>
    [Parameter] public string OnText { get; set; } = "ON";

    /// <summary>Text shown in the pill when OFF.</summary>
    [Parameter] public string OffText { get; set; } = "OFF";

    /// <summary>Show a checkmark next to ON.</summary>
    [Parameter] public bool ShowCheckmark { get; set; } = true;

    /// <summary>Provide an icon via css class (recommended: background-image or sprite).</summary>
    [Parameter] public string? IconCss { get; set; }

    /// <summary>Alternative to IconCss: arbitrary render fragment (emoji, svg, component).</summary>
    [Parameter] public RenderFragment? IconContent { get; set; }

    /// <summary>Show the info button.</summary>
    [Parameter] public bool ShowInfo { get; set; }

    /// <summary>Info click handler (opens modal, toast, etc).</summary>
    [Parameter] public EventCallback OnInfo { get; set; }

    /// <summary>Optional aria label override.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    private string AriaLabelComputed =>
        !string.IsNullOrWhiteSpace(AriaLabel)
            ? AriaLabel!
            : $"{Title}. Currently {(Value ? "On" : "Off")}. Tap to toggle.";

    private async Task HandleToggleAsync()
    {
        if (Disabled)
        {
            return;
        }

        var newValue = !Value;

        // Update bindable value first
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(newValue);
        }

        // Then call optional side-effect hook
        if (OnToggled.HasDelegate)
        {
            await OnToggled.InvokeAsync(newValue);
        }
    }

    private async Task HandleInfoAsync()
    {
        if (OnInfo.HasDelegate)
        {
            await OnInfo.InvokeAsync();
        }
    }
}