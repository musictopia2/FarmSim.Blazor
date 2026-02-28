namespace FarmSim.Blazor.Components.Trees;
public partial class TreesComponent
{
    private TimeSpan? _unlimitedSpeedSeedTime;
    protected override void OnInitialized()
    {
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        base.OnInitialized();
    }
    protected override Task OnTickAsync()
    {
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        return base.OnTickAsync();
    }
}