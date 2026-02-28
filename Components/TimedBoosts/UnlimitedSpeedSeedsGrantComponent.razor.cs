namespace FarmSim.Blazor.Components.TimedBoosts;
public partial class UnlimitedSpeedSeedsGrantComponent(IToast toast, IntegerActionPickerService actionPickerService) : IDisposable
{
    [Parameter, EditorRequired] public EnumItemCategory Category { get; set; }
    //if there is no time remaining, then this would most likely be disposed.
    [Parameter, EditorRequired] public TimeSpan TimeRemaining { get; set; }
    private string _currentOption = "";

    private int _lastLevel = -1;
    private EnumItemCategory _lastCategory;

    protected override void OnParametersSet()
    {
        int level = ProgressionManager.CurrentLevel;

        if (level != _lastLevel || Category != _lastCategory)
        {
            var list = ItemManager.GetEligibleItems(level);
            _items = list.Where(x => x.Category == Category)
                         .ToBasicList();

            _lastLevel = level;
            _lastCategory = Category;
        }

        base.OnParametersSet();
    }
    void Cleanup()
    {
        _currentOption = "";
        actionPickerService.CompletedAsync = null;
    }
    private void ChooseInstance(ItemPlanModel instance)
    {
        _currentOption = instance.ItemName;

        int maxValue = int.MaxValue;

        actionPickerService.CompletedAsync = async howMany =>
        {
            if (howMany <= 0)
            {
                toast.ShowUserErrorToast("Enter a number greater than 0.");
                return false; // keep open
            }
            if (InventoryManager.CanAdd(_currentOption, howMany) == false)
            {
                toast.ShowUserErrorToast("Unable to add because ran out of space");
                return false;
            }

            GrantableItem grant = new()
            {
                Amount = howMany,
                Category = Category,
                Item = _currentOption,
                Source = instance.Source
            };

            if (RulesManager.AutomationEnabled == false)
            {
                switch (Category)
                {
                    case EnumItemCategory.Tree:
                        TreeManager.GrantUnlimitedTreeItems(grant);
                        break;
                    case EnumItemCategory.Crop:
                        CropManager.GrantUnlimitedCropItems(grant);
                        break;
                    case EnumItemCategory.Animal:
                        AnimalManager.GrantUnlimitedAnimalItems(grant);
                        break;
                    default:
                        toast.ShowUserErrorToast("Not supported.");
                        break;
                }
            }
            else
            {
                switch (Category)
                {
                    case EnumItemCategory.Tree:
                        TreeAutomationManager.GrantUnlimitedTreeItems(grant);
                        break;
                    case EnumItemCategory.Crop:
                        CropAutomationManager.GrantUnlimitedCropItems(grant);
                        break;
                    case EnumItemCategory.Animal:
                        AnimalAutomationManager.GrantUnlimitedAnimalItems(grant);
                        break;
                    default:
                        toast.ShowUserErrorToast("Not supported.");
                        break;
                }
            }
            Cleanup();
            return true;
        };

        actionPickerService.Pick(maxValue, BuildSideContent(_currentOption));
    }

    public void Dispose()
    {
        // 1) Force the UI-close through the pipeline that actually works in your app
        actionPickerService.UpdateVisibleStatus(false);

        // 2) Clear the handler/content so nothing can still grant after expiry
        Cleanup();

        GC.SuppressFinalize(this);
    }

    private BasicList<ItemPlanModel> _items = [];

}