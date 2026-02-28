namespace FarmSim.Blazor.Components.SpeedSeeds;
public partial class SpeedSeedComponent(IToast toast)
{
    //for quite a while, a person has to do one at a time.
    //if i do allow a custom amount (that is later).

    private readonly BasicList<GrantableItem> _list = [];

    private bool _showAnimals;


    private void ShowAnimals()
    {
        _showAnimals = true;
    }
    protected override void OnInitialized()
    {
        if (RulesManager.AutomationEnabled == false)
        {
            _list.AddRange(TreeManager.GetUnlockedTreeGrantItems());
            _list.AddRange(CropManager.GetUnlockedCropGrantItems());
        }
        else
        {
            _list.AddRange(TreeAutomationManager.GetUnlockedTreeGrantItems());
            _list.AddRange(CropAutomationManager.GetUnlockedCropGrantItems());
        }
        
        //doing trees first.
        base.OnInitialized();
    }
    private static string DisplayAmount(GrantableItem item) => $"+ {item.Amount}";
    private void TryToUse(GrantableItem item)
    {
        if (InventoryManager.CanAdd(item) == false)
        {
            
            toast.ShowUserErrorToast($"Silot is full. Unable to add {item.Item}");
            return;
        }
        if (InventoryManager.Has(CurrencyKeys.SpeedSeed, 1) == false)
        {
            toast.ShowUserErrorToast("You have no more speed seeds left");
            return;
        }
        if (RulesManager.AutomationEnabled == false)
        {
            if (item.Category == EnumItemCategory.Tree)
            {
                TreeManager.GrantTreeItems(item, 1);
                return;
            }
            if (item.Category == EnumItemCategory.Crop)
            {
                CropManager.GrantCropItems(item, 1);
                return;
            }
        }
        else
        {
            if (item.Category == EnumItemCategory.Tree)
            {
                TreeAutomationManager.GrantTreeItems(item, 1);
                return;
            }
            if (item.Category == EnumItemCategory.Crop)
            {
                CropAutomationManager.GrantCropItems(item, 1);
                return;
            }
        }
        toast.ShowWarningToast("Not supported");

        //toast.ShowInfoToast("Trying to use speed seed");
    }


}