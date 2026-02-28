namespace FarmSim.Blazor.Components.Upgrades;
public partial class ManualUpgradeTreeModal(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public TreeView Tree { get; set; }
    [Parameter]
    public EventCallback OnUpgraded { get; set; }

    private readonly BasicList<UpgradeColumn> _columns = [];
    int _personalLevel;
    int _treeLevel;
    bool _isFast;
    int _maxLevels;
    protected override void OnInitialized()
    {
        //do a loopup to get the details.
        //start with 2.
        _maxLevels = UpgradeManager.MaximumBasicLevels;
        int currentLevel = UpgradeManager.TreeCurrentLevel(Tree);
        _personalLevel = ProgressionManager.CurrentLevel;
        _treeLevel = TreeManager.GetLevel(Tree);
        _isFast = TreeManager.IsFast(Tree);

        _maxLevels.Times(x =>
        {
            if (x > 1)
            {
                var costs = UpgradeManager.GetBasicItemsUpgradeCost(x);
                UpgradeColumn column = new(x, TreeManager.LevelRequiredForUpgrade(Tree, x), costs);

                _columns.Add(column);
            }
        });
        base.OnInitialized();
    }
    private string GetFaster(int level) => UpgradeManager.GetBasicPercent(level, _isFast);
    private bool CanUpgrade => UpgradeManager.CanUpgradeTreeLevel(Tree);
    private void Upgrade()
    {
        if (CanUpgrade == false)
        {
            toast.ShowUserErrorToast("Unable to upgrade");
            return;
        }
        UpgradeManager.UpgradeTreeLevel(Tree);
        OnUpgraded.InvokeAsync();
    }
}