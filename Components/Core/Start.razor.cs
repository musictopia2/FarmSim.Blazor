namespace FarmSim.Blazor.Components.Core;
public partial class Start(NavigationManager nav, IStartFarmRegistry starts, IPlayerNamePresenter playerName)
{
    private void NavigateTo(string farmTheme, string person, string mode, EnumFarmSlot slot)
    {
        nav.NavigateTo($"/farm/{farmTheme}/{person}/{mode}/{slot}");
    }

    private BasicList<FarmKey> _farms = [];
    protected override async Task OnInitializedAsync()
    {
        _farms = await starts.GetFarmsAsync();
        _farms.KeepConditionalItems(x => x.Slot == EnumFarmSlot.Main);

    }
}