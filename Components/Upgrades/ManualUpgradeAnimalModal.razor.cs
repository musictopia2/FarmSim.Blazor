namespace FarmSim.Blazor.Components.Upgrades;
public partial class ManualUpgradeAnimalModal(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public AnimalView Animal { get; set; }
    [Parameter]
    public EventCallback OnUpgraded { get; set; }
    private readonly BasicList<UpgradeColumn> _columns = [];
    int _personalLevel;
    int _animalLevel;
    bool _isFast;
    int _maxLevels;
    protected override void OnInitialized()
    {
        //do a loopup to get the details.
        //start with 2.
        _maxLevels = UpgradeManager.MaximumBasicLevels;
        int currentLevel = UpgradeManager.AnimalCurrentLevel(Animal);
        _personalLevel = ProgressionManager.CurrentLevel;
        _animalLevel = AnimalManager.GetLevel(Animal);
        _isFast = AnimalManager.IsFast(Animal);

        _maxLevels.Times(x =>
        {
            if (x > 1)
            {
                var costs = UpgradeManager.GetBasicItemsUpgradeCost(x);
                UpgradeColumn column = new(x, AnimalManager.LevelRequiredForUpgrade(Animal, x), costs);

                _columns.Add(column);
            }
        });
        base.OnInitialized();
    }
    private string GetFaster(int level) => UpgradeManager.GetBasicPercent(level, _isFast);
    private bool CanUpgrade => UpgradeManager.CanUpgradeAnimalLevel(Animal);
    private void Upgrade()
    {
        if (CanUpgrade == false)
        {
            toast.ShowUserErrorToast("Unable to upgrade");
            return;
        }
        UpgradeManager.UpgradeAnimalLevel(Animal);
        OnUpgraded.InvokeAsync();
    }
}