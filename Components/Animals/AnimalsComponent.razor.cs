namespace FarmSim.Blazor.Components.Animals;
public partial class AnimalsComponent : IDisposable
{
    private TimeSpan? _unlimitedSpeedSeedTime;
    override protected void OnInitialized()
    {
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        TimedBoostManager.Tick += UpdateUnlimitedSpeedSeeds;
    }
    private void UpdateUnlimitedSpeedSeeds()
    {
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        InvokeAsync(StateHasChanged);
    }
    public void Dispose()
    {
        TimedBoostManager.Tick -= UpdateUnlimitedSpeedSeeds;
        GC.SuppressFinalize(this);
    }
}