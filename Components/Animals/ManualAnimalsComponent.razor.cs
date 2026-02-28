namespace FarmSim.Blazor.Components.Animals;

public partial class ManualAnimalsComponent
{
    private BasicList<AnimalView> _animals = [];
    private TimeSpan? _unlimitedSpeedSeedTime;
    private bool _upgradeMode;
    private bool _anyAdvancedUpgrades;
    private void ToggleUpgradeMode()
    {
        _upgradeMode = !_upgradeMode;
    }
    override protected void OnInitialized()
    {
        AnimalManager.OnAnimalsUpdated += Refresh;
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        TimedBoostManager.Tick += UpdateUnlimitedSpeedSeeds;
        _anyAdvancedUpgrades = UpgradeManager.HasAdvancedUpgrades;
        if (this.IsCooperative)
        {
            _anyAdvancedUpgrades = false;
        }
        UpdateAnimals();

    }
    private void Upgraded()
    {
        _upgradeMode = false;
    }
    private void UpdateAnimals()
    {
        _animals = AnimalManager.GetUnlockedAnimals;
    }
    private void UpdateUnlimitedSpeedSeeds()
    {
        _unlimitedSpeedSeedTime = TimedBoostManager.GetUnlimitedSpeedSeedTimeLeft();
        InvokeAsync(StateHasChanged);
    }

    private void Refresh()
    {
        UpdateAnimals();
        InvokeAsync(StateHasChanged);
    }
    public void Dispose()
    {
        AnimalManager.OnAnimalsUpdated -= Refresh;
        TimedBoostManager.Tick -= UpdateUnlimitedSpeedSeeds;
        GC.SuppressFinalize(this);
    }
}