namespace FarmSim.Blazor.Components.Workshops;

public partial class ManualWorkshopComponent(IToast toast, FarmTransferService transfer)
{
    [Parameter]
    [EditorRequired]
    public WorkshopView Workshop { get; set; }

    [Parameter]
    public EventCallback RentalExpired { get; set; }


    private BasicList<WorkshopRecipeSummary> _recipes = [];

    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }
    [Parameter]
    public EventCallback<string> NavigateTo { get; set; }
    private int _capacity;
    private bool _showToast = true;
    private bool _showPowerGloves = false;

    private bool _showConfirmation = false;
    private bool _showUpgrades = false;
    private bool _raisedEvent;
    private string _lastRentalText = "";
    private bool _anyAdvancedUpgrades;
    private bool _showDecisionui = false;

    //private string RentalTimeLeft => RentalManager.GetDurationString(Workshop.Name);

    private void StartDecision()
    {
        _showDecisionui = true;
    }
    private void BulkPowerGloves()
    {
        _showDecisionui = false;
        if (InventoryManager.Has(CurrencyKeys.PowerGloveWorkshop, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have any power gloves.  Try purchasing more");
            return;
        }
        WorkshopManager.CompleteWorkshopJobsBulkUsingPowerGloves(Workshop);
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
        WorkshopManager.CompleteWorkshopJobsBulkUsingFinishSingle(Workshop);
        toast.ShowSuccessToast("Successfully used several complete single workshop consumables to complete several workshop jobs"); ;
    }

    private string GetConfirmationMessage =>
        $"Are you sure you want to use the finish single workshop to complete this?  You have {InventoryManager.Get(CurrencyKeys.FinishSingleWorkshop)} left";
    private void ConfirmCompleteWorkshop(bool allowed)
    {
        _showConfirmation = false;
        if (allowed)
        {
            WorkshopManager.CompleteSingleActiveJobImmediately(Workshop);
            return;
        }
    }
    private void ShowUpgrades()
    {
        _showUpgrades = true;
    }
    private void Upgraded()
    {
        _showUpgrades = false;
        RebuildWorkshopUi();
    }
    private string WorkshopDetails
    {
        get
        {
            if (_anyAdvancedUpgrades == false)
            {
                return Workshop.Name.GetWords;
            }
            return $"{Workshop.Name.GetWords} Level {WorkshopManager.GetLevel(Workshop)}";
        }
    }
    private void ClosePowerGloves()
    {
        _showPowerGloves = false;
    }
    private void RebuildWorkshopUi()
    {
        _anyAdvancedUpgrades = UpgradeManager.HasAdvancedUpgrades;
        if (this.IsCooperative)
        {
            _anyAdvancedUpgrades = false; //no matter what because its coop.
        }
        _showToast = true;
        _raisedEvent = false;

        _recipes = WorkshopManager.GetRecipesForWorkshop(Workshop);

        var extra = _recipes.FirstOrDefault(x => x.Unlocked == false);
        _recipes.RemoveAllAndObtain(x => x.Unlocked == false);
        if (extra is not null)
        {
            _recipes.Add(extra);
        }

        if (_recipes.Count > 0)
        {
            Workshop.SelectedRecipeIndex = Math.Clamp(
                Workshop.SelectedRecipeIndex, 0, _recipes.Count - 1
            );
        }

        _capacity = WorkshopManager.GetCapcity(Workshop);
    }
    protected override void OnParametersSet()
    {
        RebuildWorkshopUi();

        base.OnParametersSet();
    }
    private bool CanShowPossibleNewCapacity => UpgradeManager.IsWorkshopAtMaximumCapacity(Workshop) == false && this.IsCooperative == false;
    private void UpgradeWorkshopCapacity()
    {
        if (UpgradeManager.CanUpgradeWorkshopCapacity(Workshop) == false)
        {
            toast.ShowUserErrorToast("Unable to upgrade the workshop capacity because you don't have enough coin");
            return;
        }
        UpgradeManager.UpgradeWorkshopCapacity(Key, Workshop, transfer);
    }

    private WorkshopRecipeSummary CurrentRecipe => _recipes[Workshop.SelectedRecipeIndex];
    private string ChosenItem => CurrentRecipe.Item;
    private bool CanCraft => WorkshopManager.CanCraft(Workshop, ChosenItem);
    private void CraftSingle()
    {
        if (CurrentRecipe.Unlocked == false)
        {
            return; //cannto craft because its locked
        }
        if (CanCraft == false)
        {
            return;
        }
        WorkshopManager.StartCraftingJob(Workshop, ChosenItem);
    }
    private void CraftBulk()
    {

        if (CurrentRecipe.Unlocked == false)
        {
            return; //cannto craft because its locked
        }

        int queued = WorkshopManager.CraftBulkAcrossSameWorkshops(
         Workshop,
         ChosenItem,
         Workshop.SelectedRecipeIndex);

        if (queued == 0)
        {
            toast.ShowInfoToast("Nothing crafted (not enough ingredients and/or queues are full)");
            return;
        }

        toast.ShowSuccessToast($"Queued {queued} {ChosenItem.GetWords} across all {Workshop.Name.GetWords} workshops");
        RebuildWorkshopUi();


        //toast.ShowInfoToast("Starting Craft Bulk");
    }
    private int CurrentAmount => InventoryManager.GetInventoryCount(ChosenItem);
    private string DetailText
    {
        get
        {
            if (CurrentRecipe.Unlocked)
            {
                return CurrentRecipe.Duration.GetTimeString;
            }
            return $"Level {ProgressionManager.LevelForCraftedItem(CurrentRecipe.Item)}";
        }
    }
    private bool CanGoUp => Workshop.SelectedRecipeIndex > 0;
    private bool CanGoDown => Workshop.SelectedRecipeIndex < _recipes.Count - 1;

    private void GoUp()
    {
        Workshop.SelectedRecipeIndex--;
        WorkshopManager.UpdateSelectedRecipe(Workshop, Workshop.SelectedRecipeIndex);
    }

    private void GoDown()
    {
        Workshop.SelectedRecipeIndex++;
        WorkshopManager.UpdateSelectedRecipe(Workshop, Workshop.SelectedRecipeIndex);
    }
    protected override Task OnTickAsync()
    {
        if (_raisedEvent)
        {
            return base.OnTickAsync();
        }
        if (Workshop.IsRental)
        {
            if (WorkshopManager.Unlocked(Workshop) == false)
            {
                RentalExpired.InvokeAsync();
                _raisedEvent = true;
                return base.OnTickAsync();
            }
        }

        if (Workshop.IsRental)
        {
            _lastRentalText = RentalManager.GetDurationString(Workshop.Name);
        }
        else
        {
            _lastRentalText = ""; //try here.
        }

        _capacity = WorkshopManager.GetCapcity(Workshop);
        if (WorkshopManager.CanPickupManually(Workshop))
        {
            if (WorkshopManager.CanAddToInventory(Workshop))
            {
                WorkshopManager.PickupManually(Workshop); //since you are already there, no problem.
                _showToast = true;
            }
            else if (_showToast)
            {
                toast.ShowUserErrorToast("Unable to pick up crafted item because the barn is full.  Try discarding or consuming the items");
                _showToast = false;
            }
        }
        else if (_showToast == false)
        {
            _showToast = true;
        }
        return base.OnTickAsync();
    }

    private Dictionary<string, int> FullRequirements
    {
        get
        {
            return CurrentRecipe.Inputs;
        }
    }
    private List<CraftingSummary> GetVisibleQueue()
    {
        var list = new List<CraftingSummary>(_capacity);

        // your slots appear to be 1-based
        for (int slot = 1; slot <= _capacity; slot++)
        {
            var s = WorkshopManager.GetSingleCraftedItem(Workshop, slot);
            if (s is null)
            {
                continue;
            }

            // Hide ready-to-pickup items completely from the queue UI
            if (s.State == EnumWorkshopState.ReadyToPickUpManually)
            {
                continue;
            }
            list.Add(s);
        }
        return list;
    }
    private static string SlotClass(int displaySlot) =>
        displaySlot == 1 ? "queue-slot-active" : "queue-slot-inactive";
    private static string GetTimeText(CraftingSummary craft) => craft.ReadyTime;
    private void OnPowerGloveClicked()
    {
        _showPowerGloves = true;
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
    //hint:  when navigating to another one, does not dispose this (so that is not the answer).
    protected override void DisposeCore()
    {
        base.DisposeCore();
    }
}