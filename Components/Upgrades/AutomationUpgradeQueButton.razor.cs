namespace FarmSim.Blazor.Components.Upgrades;
public partial class AutomationUpgradeQueButton(IToast toast)
{
    [Parameter, EditorRequired] public string Target { get; set; } = "";

    [Parameter, EditorRequired]
    public int PreviousQueCount { get; set; }
    private AdvancedUpgradeAutomationDetailModel _details = new();
    private bool _opened;
    private bool _fullyUpgraded = false;
    private int ProposedNewAmount => _details.NextBenefit is null ? PreviousQueCount : _details.NextBenefit.Value + PreviousQueCount;
    protected override void OnInitialized()
    {
        _details = AdvancedUpgradeAutomationManager.GetQueUpgradeCostDetails(Target);
        base.OnInitialized();
    }
    private void OpenUpgrades()
    {
        _opened = true;
        _details = AdvancedUpgradeAutomationManager.GetQueUpgradeCostDetails(Target);
        if (_details.CostAmount == 0)
        {
            _fullyUpgraded = true;
        }
        else
        {
            _fullyUpgraded = false;
        }
    }
    private async Task PurchaseAsync()
    {
        try
        {
            if (InventoryManager.Has(CurrencyKeys.Coin, _details.CostAmount) == false)
            {
                toast.ShowUserErrorToast("You don't have enough coin to increase the que");
                return;
            }
            await AdvancedUpgradeAutomationManager.PurchaseQueUpgradeAsync(Target);
            _details = AdvancedUpgradeAutomationManager.GetQueUpgradeCostDetails(Target);
            _opened = false;
        }
        catch (Exception ex)
        {
            toast.ShowUserErrorToast(ex.Message);
        }
    }
}