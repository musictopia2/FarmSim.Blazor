using System.Threading.Tasks;

namespace FarmSim.Blazor.Components.Inventory;
public partial class InventoryDisplayComponent(IToast toast, FarmTransferService farmTransferService) : InventoryAwareComponentBase
{
    [Parameter]
    [EditorRequired]
    public EnumInventoryStorageCategory InventoryStorageCategory { get; set; }
    private BasicList<ItemAmount> _list = [];
    private string _errorMessage = "";
    private bool _showDiscard;
    private bool _showUpgrade;
    private string _item = "";
    private int _currentSize;
    private bool _showTransfer;
    private void CancelTransfer()
    {
        _showTransfer = false;
    }
    private void TranferInventoryItem(ItemAmount item)
    {
        if (item.Item != _item)
        {
            return; //can't do anyways.
        }
        _showTransfer = false;
        
        if (farmTransferService.CanTransferInventory(Key, RulesManager, item) == false)
        {
            toast.ShowUserErrorToast($"Unable to transfer because most likely the other farm did not have enough {InventoryStorageCategory} space.  Other reasons is if you chose the automated option or if its a coin farm.");
            return;
        }
        farmTransferService.TransferInventory(Key, RulesManager, item, InventoryManager);
    }
    private async Task UpgradeAsync()
    {
        _showUpgrade = false;
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Barn)
        {
            await UpgradeManager.UpgradeBarnAsync(Key, farmTransferService);
            RefreshUpgrades();
            return;
        }
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Silo)
        {
            await UpgradeManager.UpgradeSiloAsync(Key, farmTransferService);
            RefreshUpgrades();
            return;
        }
    }
    private void DiscardInventoryItem(ItemAmount item)
    {
        if (item.Item != _item)
        {
            return; //can't do anyways.
        }
        //for now, just a toast to prove everything works.
        //toast.ShowInfoToast($"So far, discarding {item.Item} with the amount of {item.Amount}");
        _showDiscard = false;

        InventoryManager.Consume(item);
        //hopefully is notified naturally anyways.

    }
    private async Task OpenUpgrade()
    {
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Barn)
        {
            if (UpgradeManager.CanUpgradeBarn == false)
            {
                toast.ShowUserErrorToast("Unable to upgrade barn because not enough coins");
                return;
            }
            
        }
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Silo)
        {
            if (UpgradeManager.CanUpgradeSilo == false)
            {
                toast.ShowUserErrorToast("Unable to upgrade silo because not enough coins");
                return;
            }
        }
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showUpgrade = true;
        }
        else
        {
            await UpgradeAsync();
        }
    }
    

    private int _newSize;
    private int _newCoinCost;
    private bool _isMaxedOut;
    
    private void CancelDiscard()
    {
        _showDiscard = false;
        _item = "";
    }
    private int _limit;
    
    protected override void OnParametersSet()
    {
        PopulateList();
        RefreshUpgrades();
        base.OnParametersSet();
    }
    private void RefreshUpgrades()
    {
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Barn)
        {
            _isMaxedOut = UpgradeManager.IsBarnMaxedOut;
            if (_isMaxedOut == false)
            {
                _newSize = UpgradeManager.NextBarnCount;
                _newCoinCost = UpgradeManager.NextBarnCoinCost;
            }
        }
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Silo)
        {
            _isMaxedOut = UpgradeManager.IsSiloMaxedOut;
            if (_isMaxedOut == false)
            {
                _newSize = UpgradeManager.NextSiloCount;
                _newCoinCost = UpgradeManager.NextSiloCoinCost;
            }
            
        }
    }
    protected override void OnInitialized()
    {
        PopulateList();

        base.OnInitialized();
    }
    private void PopulateList()
    {
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Barn)
        {
            _list = InventoryManager.GetAllBarnInventoryItems();
        }
        else if (InventoryStorageCategory == EnumInventoryStorageCategory.Silo)
        {
            _list = InventoryManager.GetAllSiloInventoryItems();
        }
        else
        {
            _errorMessage = "Can only show barn or silo items";
        }
        //figure out how to make limit changes show up (well see).
        if (InventoryStorageCategory == EnumInventoryStorageCategory.Barn)
        {
            _limit = InventoryManager.BarnSize;
        }
        else if (InventoryStorageCategory == EnumInventoryStorageCategory.Silo)
        {
            _limit = InventoryManager.SiloSize;
        }
        else
        {
            _limit = 0;
        }
        _currentSize = _list.Sum(x => x.Amount);

    }
    private void DisplayInventoryItem(string itemName)
    {
        toast.ShowInfoToast(itemName.GetWords);
    }
    private void OpenTransfer(string name)
    {
        _item = name;
        _showTransfer = true;
    }
    private void OpenDiscard(string name)
    {
        _item = name;
        _showDiscard = true;
        //toast.ShowSuccessToast($"Attempting to open discard for {name}");
    }
    protected override async Task OnInventoryChangedAsync()
    {
        PopulateList();
        await base.OnInventoryChangedAsync();
    }
    // itemName is the image file name in wwwroot (root folder).
    // If itemName already includes an extension (e.g. "barn.png"), it will use it as-is.
    // If it doesn't, it assumes ".png".
    private static string GetItemImageSrc(string itemName)
    {
        return $"{itemName}.png";
    }


    private string StorageTitle =>
        InventoryStorageCategory ==  EnumInventoryStorageCategory.Barn ? "Items In Barn" : "Items In Silo";

    // 0..1 (can be > 1 if over, we clamp for display)
    private double FillRatio =>
        _limit <= 0 ? 1 : Math.Min(1.0, (double)_currentSize / _limit);

    private string ProgressStyle => $"width:{FillRatio * 100:0.#}%";

    private string ProgressClass
    {
        get
        {
            if (_limit <= 0)
            {
                return "p-red"; // defensive
            }

            var ratio = (double)_currentSize / _limit;

            if (ratio < 0.50)
            {
                return "p-green";
            }

            if (ratio < 0.80)
            {
                return "p-orange";
            }

            return "p-red";
        }
    }

}