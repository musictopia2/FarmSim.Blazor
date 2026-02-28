namespace FarmSim.Blazor.Components.Upgrades;
public partial class ManualUpgradeCropModal(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public string CropName { get; set; }
    [Parameter]
    public EventCallback OnUpgraded { get; set; }

    private readonly BasicList<UpgradeColumn> _columns = [];
    int _personalLevel;
    int _cropLevel;
    bool _isFast;
    int _maxLevels;
    protected override void OnInitialized()
    {
        //do a loopup to get the details.
        //start with 2.
        _maxLevels = UpgradeManager.MaximumBasicLevels;
        int currentLevel = UpgradeManager.CropCurrentLevel(CropName);
        _personalLevel = ProgressionManager.CurrentLevel;
        _cropLevel = CropManager.GetLevel(CropName);
        _isFast = CropManager.IsFast(CropName);

        _maxLevels.Times(x =>
        {
            if (x > 1)
            {
                var costs = UpgradeManager.GetBasicItemsUpgradeCost(x);
                UpgradeColumn column = new(x, CropManager.LevelRequiredForUpgrade(CropName, x), costs);

                _columns.Add(column);
            }
        });

        base.OnInitialized();
    }

    private string GetFaster(int level) => UpgradeManager.GetBasicPercent(level, _isFast);
    private bool CanUpgrade => UpgradeManager.CanUpgradeCropLevel(CropName);
    private void Upgrade()
    {
        if (CanUpgrade == false)
        {
            toast.ShowUserErrorToast("Unable to upgrade");
            return;
        }
        UpgradeManager.UpgradeCropLevel(CropName);
        OnUpgraded.InvokeAsync();
    }
}