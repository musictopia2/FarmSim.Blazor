namespace FarmSim.Blazor.Components.Upgrades;
public partial class ManualUpgradeWorkshopModal(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public WorkshopView Workshop { get; set; }
    [Parameter]
    public EventCallback OnUpgraded { get; set; }
    private readonly BasicList<UpgradeColumn> _columns = [];
    int _personalLevel;
    int _workshopLevel;
    int _maxLevels;
    protected override void OnInitialized()
    {
        //do a loopup to get the details.
        //start with 2.
        _maxLevels = UpgradeManager.MaximumWorkshopLevels;
        _personalLevel = ProgressionManager.CurrentLevel;
        _workshopLevel = WorkshopManager.GetLevel(Workshop);

        _maxLevels.Times(x =>
        {
            if (x > 1)
            {
                var costs = UpgradeManager.GetWorkshopItemsUpgradeCost(x);
                UpgradeColumn column = new(x, UpgradeManager.WorkshopNextLevelRequirement(Workshop, x), costs);

                _columns.Add(column);
            }
        });
        base.OnInitialized();
    }
    private string GetFaster(int level) => UpgradeManager.GetWorkshopPercent(level);
    private bool CanUpgrade => UpgradeManager.CanUpgradeWorkshopLevel(Workshop);
    private void Upgrade()
    {
        if (CanUpgrade == false)
        {
            toast.ShowUserErrorToast("Unable to upgrade");
            return;
        }
        UpgradeManager.UpgradeWorkshopLevel(Workshop);
        OnUpgraded.InvokeAsync();
    }
}