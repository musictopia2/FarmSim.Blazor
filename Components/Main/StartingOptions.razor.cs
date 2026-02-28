namespace FarmSim.Blazor.Components.Main;
public partial class StartingOptions
{
    [Parameter]
    public EventCallback<EnumRuleCategory> OnSave { get; set; }
}