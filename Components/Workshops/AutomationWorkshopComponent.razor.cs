namespace FarmSim.Blazor.Components.Workshops;
public partial class AutomationWorkshopComponent(IToast toast)
{
    [Parameter, EditorRequired]
    public WorkshopAutomationView Workshop { get; set; } = default!;

    [Parameter]
    public EventCallback<string> NavigateTo { get; set; }
    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }


    private BasicList<string> _items = [];
    private readonly Dictionary<string, int> _outstandingByItem = new(StringComparer.OrdinalIgnoreCase);

    private string? _selectedItem;

    private bool _processing;

    private bool _showPicker;
    private int _pickerLimit;
    private string _pickerVerb = "";

    private int _cap;
    private int _outstanding;
    private int _remaining;
    private bool _blocked;
    private string _nextReadyDisplay = "";
    private bool _showPowerGloves = false;
    private bool _showConfirmation = false;
    private bool _showDecisionui = false;
    private int _virtualCount;


    private string? _priorityItem;
    private bool IsPrioritySelectedItem =>
        _priorityItem is not null &&
        _priorityItem.Equals(_selectedItem, StringComparison.OrdinalIgnoreCase);

    private string BuildingDetails
    {
        get
        {
            if (IsPrioritySelectedItem == false)
            {
                return Workshop.BuildingName;
            }
            return $"{Workshop.BuildingName} {_priorityItem}";
        }
    }

    private string GetConfirmationMessage =>
        $"Are you sure you want to use the finish single workshop to complete this?  You have {InventoryManager.Get(CurrencyKeys.FinishSingleWorkshop)} left";
    private Dictionary<string, int> FullRequirements
    {
        get
        {
            if (_selectedItem is null)
            {
                return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            }
            return WorkshopAutomationManager.GetFullRequirements(_selectedItem);
        }
    }
    protected override void OnParametersSet()
    {
        _selectedItem = Workshop.SelectedItem;
        RefreshItems();
        RefreshComputedState();
        base.OnParametersSet();
    }

    protected override Task OnTickAsync()
    {
        RefreshItems();
        RefreshComputedState();
        return base.OnTickAsync();
    }

    private async Task MakePriority()
    {
        WorkshopAutomationManager.SetPriorityItem(Workshop.BuildingName, _selectedItem);
        RefreshItems();
    }

    private async Task ClearPriority()
    {
        WorkshopAutomationManager.SetPriorityItem(Workshop.BuildingName, null);
        RefreshItems();
    }

    private void RefreshItems()
    {
        if (string.IsNullOrWhiteSpace(Workshop?.BuildingName))
        {
            _items = [];
            return;
        }
        _virtualCount = WorkshopAutomationManager.GetVirtualCount(Workshop.BuildingName);
        _items = WorkshopAutomationManager.GetUnlockedCraftedItemsForBuilding(Workshop.BuildingName);

        _outstandingByItem.Clear();
        foreach (var item in _items)
        {
            _outstandingByItem[item] = WorkshopAutomationManager.GetOutstanding(item);
        }

        // if selected item is no longer valid (lock/unlock changes), drop back to list
        if (_selectedItem is not null && _items.Any(x => x.Equals(_selectedItem, StringComparison.OrdinalIgnoreCase)) == false)
        {
            _selectedItem = null;
        }
        _priorityItem = WorkshopAutomationManager.GetPriorityItem(Workshop.BuildingName);
    }

    private void SelectItem(string item)
    {
        _selectedItem = item;
        RefreshComputedState();
    }

    private void BackToItems()
    {
        _selectedItem = null;
        _showPicker = false;
    }

    private void ConfirmCompleteWorkshop(bool allowed)
    {
        _showConfirmation = false;
        if (allowed)
        {
            WorkshopAutomationManager.CompleteSingleActiveJobImmediately(Workshop);
            return;
        }
    }
    private void BulkPowerGloves()
    {
        _showDecisionui = false;
        if (InventoryManager.Has(CurrencyKeys.PowerGloveWorkshop, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have any power gloves.  Try purchasing more");
            return;
        }
        WorkshopAutomationManager.CompleteWorkshopJobsBulkUsingPowerGloves(Workshop);
        toast.ShowSuccessToast("Successfully used several power gloves to complete as many jobs as possible");
    }
    private void BulkSingleWorkshop()
    {
        _showDecisionui = false;
        if (InventoryManager.Has(CurrencyKeys.FinishSingleWorkshop, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have enough Finish Single Workshop consumable to do this.  Try purchasing more");
            return;
        }
        WorkshopAutomationManager.CompleteWorkshopJobsBulkUsingFinishSingle(Workshop);
        toast.ShowSuccessToast("Successfully used several complete single workshop consumables to complete several workshop jobs"); ;
    }
    private void StartDecision()
    {
        _showDecisionui = true;
    }
    private void OnFinishNowClicked()
    {
        if (InventoryManager.Has(CurrencyKeys.FinishSingleWorkshop, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have enough Finish Single Workshop consumable to do this.  Try purchasing more");
            return;
        }
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showConfirmation = true;
        }
        else
        {
            ConfirmCompleteWorkshop(true);
        }
    }

    private void OnPowerGloveClicked()
    {
        _showPowerGloves = true;
    }
    private void ClosePowerGloves()
    {
        _showPowerGloves = false;
    }
    private void RefreshComputedState()
    {
        if (_selectedItem is null)
        {
            return;
        }

        _cap = WorkshopAutomationManager.GetCap(_selectedItem);
        _outstanding = WorkshopAutomationManager.GetOutstanding(_selectedItem);
        _remaining = WorkshopAutomationManager.GetRemainingCapacity(_selectedItem);
        _blocked = WorkshopAutomationManager.IsBlocked(_selectedItem);
        
        var next = WorkshopAutomationManager.GetTimeUntilNextReady(_selectedItem);
        if (next is null)
        {
            _processing = false;
        }
        else
        {
            _processing = true;
        }
        _nextReadyDisplay =
            _blocked ? "Blocked" :
            next is null ? "—" :
            next.Value == TimeSpan.Zero ? "Ready now" :
            next.Value.GetTimeString;
    }

    private void OpenRequestPicker()
    {
        if (_selectedItem is null)
        {
            return;
        }

        _pickerVerb = "Request";
        _pickerLimit = _cap;
        _showPicker = true;
    }

    private void OpenAddPicker()
    {
        if (_selectedItem is null)
        {
            return;
        }

        _pickerVerb = "Add";
        _pickerLimit = _remaining;
        _showPicker = true;
    }

    private Task ClosePicker()
    {
        _showPicker = false;
        return Task.CompletedTask;
    }

    private Task SubmitPickerAsync(int amount)
    {
        _showPicker = false;

        if (_selectedItem is not null)
        {
            WorkshopAutomationManager.Request(_selectedItem, amount);
        }

        RefreshItems();
        RefreshComputedState();
        return InvokeAsync(StateHasChanged);
    }
    private void NextRecipe()
    {
        if (_selectedItem is null)
        {
            return;
        }
        int index = _items.IndexOf(_selectedItem);
        if (index == _items.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        _selectedItem = _items[index];
    }
    private void PreviousRecipe()
    {
        if (_selectedItem is null)
        {
            return;
        }
        int index = _items.IndexOf(_selectedItem);
        if (index == 0)
        {
            index = _items.Count - 1;
        }
        else
        {
            index--;
        }
        _selectedItem = _items[index];
    }

}