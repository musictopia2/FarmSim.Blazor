namespace FarmSim.Blazor.Components.Upgrades;
public partial class AutomationUpgradeVirtualCountButton(IToast toast)
{
    [Parameter, EditorRequired] public string Target { get; set; } = "";

    [Parameter, EditorRequired]
    public int PreviousVirtualCount { get; set; }
    private AdvancedUpgradeAutomationDetailModel _details = new();
    private bool _opened;
    private bool _fullyUpgraded = false;
    private int ProposedNewAmount => _details.NextBenefit is null ? PreviousVirtualCount : _details.NextBenefit.Value + PreviousVirtualCount;
    protected override void OnInitialized()
    {
        _details = AdvancedUpgradeAutomationManager.GetVirtualUpgradeCostDetails(Target);
        base.OnInitialized();
    }
    private void OpenUpgrades()
    {
        _opened = true;
        _details = AdvancedUpgradeAutomationManager.GetVirtualUpgradeCostDetails(Target);
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
        if (InventoryManager.Has(CurrencyKeys.Coin, _details.CostAmount) == false)
        {
            toast.ShowUserErrorToast("You don't have enough coin to increase the virtual count");
            return;
        }
        await AdvancedUpgradeAutomationManager.PurchaseVirtualCountUpgradeAsync(Target);
        _details = AdvancedUpgradeAutomationManager.GetVirtualUpgradeCostDetails(Target);
        if (_details.CostAmount == 0)
        {
            _fullyUpgraded = true;
        }
        _opened = false;
    }
}