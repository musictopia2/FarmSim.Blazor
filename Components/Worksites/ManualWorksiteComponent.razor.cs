using BasicBlazorLibrary.Components.Layouts;
using BasicBlazorLibrary.Components.MediaQueries.ParentClasses;
namespace FarmSim.Blazor.Components.Worksites;
public partial class ManualWorksiteComponent(IToast toast, OverlayService overlay)
{

    [Parameter]
    [EditorRequired]
    public string Location { get; set; }

    [CascadingParameter]
    public MediaQueryListComponent? Query1 { get; set; }
    [CascadingParameter]
    public OverlayInsets? Insets { get; set; }

    [Parameter]
    public EventCallback OnCloseManually { get; set; } //this is when this would tell something to close.

    private BasicList<ItemAmount> _rewards = [];
    private BasicList<WorkerRecipe> _workers = [];
    private WorkerRecipe? _currentWorker;
    private BasicList<WorkerRecipe> _shownList = [];

    private string _height = "";

    private const string _ringSize = "100px";

    private bool _showPowerGloves = false;

    private bool _showCompleteConfirmation = false;
    private string? _currentRentalTime;
    private bool _showFocusedConfirmation = false;
    private string GetCompleteConfirmationMessage =>
        $"Are you sure you want to use the finish single worksite to complete this?  You have {InventoryManager.Get(CurrencyKeys.FinishSingleWorksite)} left";
    private void ConfirmCompleteWorksite(bool allowed)
    {
        _showCompleteConfirmation = false;
        if (allowed)
        {
            WorksiteManager.CompleteSingleWorksiteImmediately(Location);
            //WorkshopManager.CompleteSingleActiveJobImmediately(Workshop);
            return;
        }
    }
    private void ConfirmFocusedRun(bool allowed)
    {
        _showFocusedConfirmation = false;
        if (allowed)
        {
            WorksiteManager.ResetToFocused(Location);
        }
    }

    private void ClosePowerGloves()
    {
        _showPowerGloves = false;
    }
    private void OnPowerGloveClicked()
    {
        _showPowerGloves = true;
    }
    private void OnFinishSingleClicked()
    {
        if (InventoryManager.Has(CurrencyKeys.FinishSingleWorksite, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have enough Finish Single Worksite consumable to do this.  Try purchasing more");
            return;
        }
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showCompleteConfirmation = true;
        }
        else
        {
            ConfirmCompleteWorksite(true);
        }
    }

    //private bool _showWorkshops;

    //private EnumWorksiteCollectionMode _mode;
    private int _totalPossibleWorkers;
    private string WorksiteImageUrl => $"/{Location}.png";

    //private WorkshopView? _currentWorkshop;
    bool CanResetToFocused =>
    WorksiteManager.CanResetToFocused(Location);
    void ResetToFocused()
    {
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showFocusedConfirmation = true;
        }
        else
        {
            ConfirmFocusedRun(true);
        }
        //WorksiteManager.ResetToFocused(Location);
    }
    private async Task AttemptWorkshopAsync(string name)
    {

        var workshop = WorkshopManager.SearchForWorkshop(name);
        if (workshop is not null)
        {
            await overlay.OpenWorkshopAsync(workshop);
        }

    }

    protected override void OnInitialized()
    {
        _workers = WorksiteManager.GetUnlockedWorkers(Location);
        _currentWorker = _workers.FirstOrDefault();
        if (_currentWorker is not null)
        {
            _currentRentalTime = RentalManager.GetDurationString(_currentWorker.WorkerName);
        }
        _totalPossibleWorkers = WorksiteManager.TotalWorkersAllowed(Location);
        _shownList = GetOrderedWorkers();
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        await RunProcessAsync();
        int realHeight = Query1!.BrowserInfo!.Height;
        int taken = Insets!.TopPx + Insets!.BottomPx;
        int available = realHeight - taken;
        _height = $"{available}px";
    }

    private EnumWorksiteState Status
    {
        get
        {
            var result = WorksiteManager.GetStatus(Location);
            return result;
        }
    }
    private int _workerIndex = 0;
    private bool CanGoUp => _workerIndex > 0;
    private bool CanGoDown => _workerIndex < _workers.Count - 1;
    private void GoUp()
    {
        _workerIndex--;
        _currentWorker = _workers[_workerIndex];
        _currentRentalTime = RentalManager.GetDurationString(_currentWorker.WorkerName);
    }
    private void GoDown()
    {
        _workerIndex++;
        _currentWorker = _workers[_workerIndex];
        _currentRentalTime = RentalManager.GetDurationString(_currentWorker.WorkerName);
    }
    protected override async Task OnTickAsync()
    {
        await RunProcessAsync();
        await base.OnTickAsync();
    }
    private EnumWorksiteState _status;
    private async Task RunProcessAsync()
    {
        EnumWorksiteState status = WorksiteManager.GetStatus(Location);
        if (status == EnumWorksiteState.None && _status == EnumWorksiteState.Processing)
        {
            _shownList = GetOrderedWorkers();
        }
        _status = status;
        if (status == EnumWorksiteState.Collecting && _rewards.Count == 0)
        {
            _rewards = WorksiteManager.GetRewards(Location);

            WorksiteManager.StoreRewards(Location, _rewards);
            //even if you cannot collect. needs to store it.
            if (WorksiteManager.CanCollectRewardsWithLimits(Location) == false)
            {
                toast.ShowUserErrorToast("Unable to collect rewards because your barn or silo is full.");
                //overlay.SetWorksiteVisible(false);
                await overlay.CloseAllAsync();
                return;
            }
        }
        if (status != EnumWorksiteState.Collecting)
        {
            _rewards.Clear();
        }
        if (status == EnumWorksiteState.None && _currentWorker is not null)
        {
            _currentRentalTime = RentalManager.GetDurationString(_currentWorker.WorkerName);
        }
    }
    //if you cannot add worker, then disabled but can still view details about that worker.
    private static bool CanAddWorker(WorkerRecipe worker) => worker.WorkerStatus != EnumWorkerStatus.Working;
    private void CollectAllRewards()
    {
        if (WorksiteManager.CanCollectRewards(Location) == false)
        {
            return;
        }
        WorksiteManager.CollectAllRewards(Location, _rewards);
        _workerIndex = 0; //back to 0.
        OnCloseManually.InvokeAsync(); //since you collected all rewards, then close out
    }
    private void OnRewardClicked(ItemAmount r)
    {
        WorksiteManager.CollectSpecificReward(Location, r);
        if (_rewards.Count == 0)
        {
            OnCloseManually.InvokeAsync(); //because all rewards has been collected.
        }
    }
    private static string GetItemImage(string item) => $"/{item}.png";


    //for now, no background.
    private void GetPreviewDetail(WorksiteRewardPreview reward)
    {
        toast.ShowInfoToast(reward.Item.GetWords);
    }

    private void ProcessWorker()
    {
        CustomBasicException.ThrowIfNull(_currentWorker);
        if (CanAddWorker(_currentWorker) == false)
        {
            return;
        }
        if (_currentWorker.WorkerStatus == EnumWorkerStatus.Selected && _currentWorker.CurrentLocation != Location)
        {
            WorksiteManager.RemoveWorker(_currentWorker.CurrentLocation, _currentWorker);
            _currentWorker.CurrentLocation = Location;


        }
        if (_currentWorker.WorkerStatus == EnumWorkerStatus.None)
        {
            AddWorker();
            return;
        }
        if (_currentWorker.WorkerStatus == EnumWorkerStatus.Selected && _currentWorker.CurrentLocation == Location)
        {
            RemoveWorker();
            return;
        }
    }

    private static string Image(string name) => $"/{name}.png";
    private void AddWorker()
    {
        CustomBasicException.ThrowIfNull(_currentWorker);
        WorksiteManager.AddWorker(Location, _currentWorker);
        _shownList = GetOrderedWorkers();
    }
    private void RemoveWorker()
    {
        CustomBasicException.ThrowIfNull(_currentWorker);
        WorksiteManager.RemoveWorker(Location, _currentWorker);
        _shownList = GetOrderedWorkers();
    }
    private bool CanStartJob => WorksiteManager.CanStartJob(Location);
    private void StartJob()
    {
        if (string.IsNullOrWhiteSpace(Location))
        {
            return;
        }
        if (WorksiteManager.NeedsAutomateError(Location))
        {
            toast.ShowUserErrorToast("Unable to automatically send workers because all unlocked workers are busy at other worksites");
            return;
        }
        WorksiteManager.StartJob(Location);
        _shownList = GetOrderedWorkers(); //just in case i did automatically.
    }
    private string GetDurationText => WorksiteManager.GetDurationText(Location);
    private string GetProgressText => WorksiteManager.GetProgressText(Location);
    private BasicList<WorksiteRewardPreview> GetPreview() => WorksiteManager.GetPreview(Location);
    private BasicList<ItemAmount> SuppliesNeeded => WorksiteManager.SuppliesNeeded(Location);

    private BasicList<WorkerRecipe> GetOrderedWorkers()
    {
        // TODO: define what "proper" means for your game.
        // Example: only show workers that can work at this location.
        //bool IsProper(WorkerRecipe w) =>
        //    w is not null; // replace with your real rule

        BasicList<WorkerRecipe> output = [];
        _workers.ForEach(item =>
        {
            if (item.WorkerStatus == EnumWorkerStatus.Working && item.CurrentLocation == Location)
            {
                output.Add(item);
            }
            //else if (item.WorkerStatus == EnumWorkerStatus.None)
            //{
            //    output.Add(item);
            //}
            else if (item.WorkerStatus == EnumWorkerStatus.Selected && item.CurrentLocation == Location)
            {
                output.Add(item);
            }
        });

        return output;
    }

   
}