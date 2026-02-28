namespace FarmSim.Blazor.Components.Upgrades;
public partial class AutomationUpgradeReducedTimeButton(IToast toast)
{
    [Parameter, EditorRequired] public string Target { get; set; } = "";

    private AdvancedUpgradeAutomationDetailModel _details = new();
    private BasicList<int> _tabs = [];
    private bool _opened;
    private bool _fullyUpgraded;

    //private string? _activeKey = null;

    private string ActiveKey
    {
        get
        {
            int level = _details.Level;
            level++;
            return level.ToString();
        }
    }
    protected override void OnInitialized()
    {

        _details = AdvancedUpgradeAutomationManager.GetTimeReductionUpgradeCostDetails(Target);
        base.OnInitialized();
    }
    private void OpenUpgrades()
    {
        _opened = true;
        _details = AdvancedUpgradeAutomationManager.GetTimeReductionUpgradeCostDetails(Target);
        int max = _details.ReducedTimes.Count;
        _tabs = Enumerable.Range(1, max).ToBasicList();
        _fullyUpgraded = false;
        if (_tabs.Count == 0)
        {
            _fullyUpgraded = true;
        }
    }

    private async Task PurchaseAsync()
    {
        try
        {
            if (InventoryManager.Has(CurrencyKeys.Coin, _details.CostAmount) == false)
            {
                toast.ShowUserErrorToast("You don't have enough coin to reduce the time");
                return;
            }
            await AdvancedUpgradeAutomationManager.PurchaseTimeReductionUpgradeAsync(Target);
            _details = AdvancedUpgradeAutomationManager.GetTimeReductionUpgradeCostDetails(Target);
            _opened = false;
        }
        catch (Exception ex)
        {
            toast.ShowUserErrorToast(ex.Message);
        }
    }
}