namespace FarmSim.Blazor.Components.Worksites;
public partial class AutomationWorksiteComponent(OverlayService overlay, IToast toast)
{
    [Parameter]
    public string Location { get; set; } = "";

    [Parameter]
    public EventCallback OnCloseManually { get; set; }
    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }

    private const string _height = "90vh";
    private const string _ringSize = "90";

    private bool _showWorkerPicker;
    private bool _showRequestPicker;

    private int _cap;
    private int _outstanding;
    private int _remaining;
    private TimeSpan? _timeUntil;
    private string _timeText = "";

    private BasicList<WorkerRecipe> _available = [];
    private BasicList<WorkerRecipe> _pending = [];
    private BasicList<WorkerRecipe> _active = [];
    private int _maxWorkers;

    private string _requestVerb = "Request";
    private int _requestLimit;
    private bool _canOpenRequest;
    private string _requestButtonText = "Request";

    private bool _isBlocked;
    private bool _showPowerGloves = false;

    private bool _showCompleteConfirmation = false;
    private string GetCompleteConfirmationMessage =>
        $"Are you sure you want to use the finish single worksite to complete this?  You have {InventoryManager.Get(CurrencyKeys.FinishSingleWorksite)} left";
    private BasicList<ItemAmount> SuppliesNeeded => WorksiteAutomationManager.SuppliesNeeded(Location);
    private void ConfirmCompleteWorksite(bool allowed)
    {
        _showCompleteConfirmation = false;
        if (allowed)
        {
            if (RulesManager.AutomationEnabled == false)
            {
                WorksiteManager.CompleteSingleWorksiteImmediately(Location);
            }
            else
            {
                WorksiteAutomationManager.CompleteSingleWorksiteImmediately(Location);
            }
            return;
        }
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
    private void ClosePowerGloves()
    {
        _showPowerGloves = false;
    }
    private void OnPowerGloveClicked()
    {
        _showPowerGloves = true;
    }

    private int WorkersSelected => _pending.Count;

    private string GetElsewhereText(WorkerRecipe worker)
    {
        // These methods are the ones we added earlier on WorksiteAutomationManager
        var active = WorksiteAutomationManager
            .GetWorkerActiveLocations(worker.WorkerName)
            .Where(x => x.Equals(Location, StringComparison.OrdinalIgnoreCase) == false)
            .ToList();

        var planned = WorksiteAutomationManager
            .GetWorkerPlannedLocations(worker.WorkerName)
            .Where(x => x.Equals(Location, StringComparison.OrdinalIgnoreCase) == false)
            .ToList();

        if (active.Count == 0 && planned.Count == 0)
        {
            return "";
        }

        string a = active.Count > 0 ? $"Active: {string.Join(", ", active.Select(x => x.GetWords))}" : "";
        string p = planned.Count > 0 ? $"Planned: {string.Join(", ", planned.Select(x => x.GetWords))}" : "";

        if (string.IsNullOrWhiteSpace(a))
        {
            return p;
        }

        if (string.IsNullOrWhiteSpace(p))
        {
            return a;
        }

        return $"{a} • {p}";
    }
    protected override void OnParametersSet()
    {
        Refresh();
        base.OnParametersSet();
    }

    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }

    private void Refresh()
    {
        if (string.IsNullOrWhiteSpace(Location))
        {
            return;
        }
        _isBlocked = false;
        _cap = WorksiteAutomationManager.GetQueueCap(Location);
        _outstanding = WorksiteAutomationManager.GetOutstandingRuns(Location);
        _remaining = WorksiteAutomationManager.GetRemainingQueueSpace(Location);
        _timeUntil = WorksiteAutomationManager.GetTimeUntilNextReady(Location);

        _timeText = _timeUntil is null
            ? ""
            : _timeUntil.Value.GetTimeString;

        // Workers
        _active = WorksiteAutomationManager.GetAssignedWorkers(Location);
        _pending = WorksiteAutomationManager.GetPendingWorkers(Location);
        _available = WorksiteAutomationManager.GetUnlockedWorkers();
        _maxWorkers = WorksiteAutomationManager.GetMaximumWorkers(Location);

        // Request button behavior
        if (_cap <= 0)
        {
            _canOpenRequest = false;
            _requestButtonText = "Locked";
            _requestVerb = "Request";
            _requestLimit = 0;
            return;
        }

        if (_outstanding <= 0)
        {
            _requestVerb = "Request";
            _requestLimit = _cap;
            _requestButtonText = $"Request (max {_cap})";
            _canOpenRequest = _cap > 0;
        }
        else
        {
            _requestVerb = "Add";
            _requestLimit = _remaining;
            _requestButtonText = _remaining > 0
                ? $"Add (max {_remaining})"
                : "Queue Full";
            _canOpenRequest = _remaining > 0;
        }
        if (_outstanding > 0)
        {
            if (WorksiteAutomationManager.IsBlocked(Location))
            {
                _isBlocked = true;
            }
        }
    }

    private string GetHeadline
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                return "";
            }

            if (_outstanding <= 0)
            {
                return "No runs queued";
            }
            if (_isBlocked)
            {
                return "Blocked";
            }
            if (_timeUntil is null)
            {
                // Most common causes: inputs missing OR rewards waiting for inventory space.
                return "Queued – waiting";
            }

            if (_timeUntil.Value <= TimeSpan.Zero)
            {
                return "Finishing now…";
            }

            return $"Next run completes in {_timeText}";
        }
    }

    private void OpenWorkerPicker()
    {
        if (string.IsNullOrWhiteSpace(Location))
        {
            return;
        }
        _showWorkerPicker = true;
    }

    private void CloseWorkerPicker()
    {
        _showWorkerPicker = false;
    }

    private void ToggleWorker(WorkerRecipe worker)
    {
        if (string.IsNullOrWhiteSpace(Location))
        {
            return;
        }

        bool selected = _pending.Any(x => x.WorkerName.Equals(worker.WorkerName, StringComparison.OrdinalIgnoreCase));
        if (selected)
        {
            WorksiteAutomationManager.RemoveWorker(Location, worker);
        }
        else
        {
            WorksiteAutomationManager.AddWorker(Location, worker);
        }

        Refresh();
        StateHasChanged();
    }

    private void OpenRequestPicker()
    {
        if (_canOpenRequest == false)
        {
            return;
        }
        _showRequestPicker = true;
    }
    private void CloseRequestPicker()
    {
        _showRequestPicker = false;
    }
    private async Task AttemptWorkshopAsync(string name)
    {
        var workshop = WorkshopAutomationManager.SearchForWorkshop(name);
        if (workshop is not null)
        {
            await overlay.OpenWorkshopAsync(workshop);
        }
    }
    private Task RequestRunsAsync(int amount)
    {
        if (string.IsNullOrWhiteSpace(Location))
        {
            return Task.CompletedTask;
        }

        // AmountPicker already enforces <= Limit, but keep it defensive.
        if (amount <= 0)
        {
            return Task.CompletedTask;
        }

        if (_outstanding <= 0)
        {
            amount = Math.Min(amount, _cap);
        }
        else
        {
            amount = Math.Min(amount, _remaining);
        }

        WorksiteAutomationManager.RequestRuns(Location, amount);
        _showRequestPicker = false;
        Refresh();
        return Task.CompletedTask;
    }
}
