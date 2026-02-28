namespace FarmSim.Blazor.Components.Inventory;
public partial class InventoryUpgradeConfirmation
{
    [Parameter]
    [EditorRequired]
    public EnumInventoryStorageCategory StorageCategory { get; set; }

    [Parameter]
    public EventCallback OnUpgrade { get; set; }

    private bool _canUpgrade;
    private string _name = "";

    protected override void OnParametersSet()
    {
        if (StorageCategory == EnumInventoryStorageCategory.Barn)
        {
            _canUpgrade = UpgradeManager.CanUpgradeBarn;
            _name = nameof(EnumInventoryStorageCategory.Barn);
        }
        else if (StorageCategory == EnumInventoryStorageCategory.Silo)
        {
            _canUpgrade = UpgradeManager.CanUpgradeSilo;
            _name = nameof(EnumInventoryStorageCategory.Silo);
        }
        else
        {
            throw new CustomBasicException("Not supported"); //intended to be barn or silo only.
        }
        base.OnParametersSet();
    }

    

}