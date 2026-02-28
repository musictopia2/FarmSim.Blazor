namespace FarmSim.Blazor.Components.Crops;
public partial class CropsComponent
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