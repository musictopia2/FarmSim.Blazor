namespace FarmSim.Blazor.Components.Store;
public partial class ShopComponent(IToast toast, IMessageBox message, FarmTransferService transfer)
{
    private async Task OnActiveKeyChanged(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (Enum.TryParse<EnumCatalogCategory>(key, out var category))
        {
            await StoreManager.ChoseNewCategoryAsync(category);
            Reload();
        }
    }
    private BasicList<StoreItemRowModel> _list = [];
    private void Reload()
    {
        _list = StoreManager.GetStoreItems();
    }


    private readonly (EnumCatalogCategory Category, string Text)[] _tabs =
    [
        (EnumCatalogCategory.Tree,     "Trees"),
        (EnumCatalogCategory.Animal,   "Animals"),
        (EnumCatalogCategory.Workshop, "Workshops"),
        (EnumCatalogCategory.Worker,   "Workers"),
        (EnumCatalogCategory.Worksite, "Worksites"),
        (EnumCatalogCategory.InstantUnlimited, "Instant Unlimited"),
        (EnumCatalogCategory.TimedBoosts, "Timed Boosts"),
        (EnumCatalogCategory.Misc, "Misc Items")
    ];

    private StoreItemRowModel? _currentItem;
    private AdvancedSettingsProfileModel _settings = new();
    private bool _showConfirmation;
    private async Task ConfirmedAsync()
    {
        if (_currentItem is null)
        {
            return;
        }
        _showConfirmation = false;
        await StoreManager.AcquireAsync(Key, transfer ,_currentItem);
        Reload();
        toast.ShowSuccessToast($"Successfully acquired {_currentItem.TargetName.GetWords}");
        _currentItem = null;
    }

    private async Task ShowAugmentationDetailsAsync(StoreItemRowModel row)
    {
        if (string.IsNullOrWhiteSpace(row.OutputAugmentationKey))
        {
            return;
        }

        string description = Farm!.OutputAugmentationManager.GetDescription(row.OutputAugmentationKey);
        await message.ShowMessageAsync(description);
    }
    private void CancelPurchase()
    {
        _showConfirmation = false;
    }
    private async Task OnRowClicked(StoreItemRowModel row)
    {
        // You said you need the outer div for click; keep it here.
        // Example: if locked/maxed you could ignore, else attempt purchase.
        if (row.IsLocked || row.IsMaxedOut)
        {
            return;
        }
        if (StoreManager.CanAcquire(row) == false)
        {
            toast.ShowUserErrorToast("Failed to acquire."); //not always coins.
            return;
        }
        _currentItem = row;
        if (_settings.UseConfirmations)
        {
            _showConfirmation = true;
        }
        else
        {
            await ConfirmedAsync();
        }
    }

    private static string CardStateClass(StoreItemRowModel row)
    {
        if (row.IsLocked)
        {
            return "is-locked";
        }

        if (row.IsMaxedOut)
        {
            return "is-maxed";
        }

        return "is-ready";
    }



    protected override void OnInitialized()
    {
        _settings = AdvancedSettingsManager.Settings;
        Reload();
        base.OnInitialized();
    }
}