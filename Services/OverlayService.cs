namespace FarmSim.Blazor.Services;
public class OverlayService(PopupRegistry popup, FarmContext farm)
{
    public event Action? Changed;
    public IToast? Toast { get; set; }
    public bool ShowQuestOrScenarioBook { get; private set; }
    public bool ShowAnimals { get; private set; }
    public bool ShowTrees { get; private set; }
    public bool ShowCrops { get; private set;  }
    public bool ShowLevelDetails { get; private set; }
    
    public bool ShowWorkshops => CurrentWorkshop is not null || CurrentAutomatedWorkshop is not null;
    public bool ShowWorksites => CurrentWorksite is not null;
    public void Init()
    {
        if (farm.Current!.RulesManager.AutomationEnabled)
        {
            return;
        }
        farm.Current!.AnimalManager.OnAugmentedOutput += OnAugmentedOutput;
        farm.Current.CropManager.OnAugmentedOutput += OnAugmentedOutput;
        farm.Current.TreeManager.OnAugmentedOutput += OnAugmentedOutput;
        farm.Current.WorkshopManager.OnAugmentedOutput += OnAugmentedOutput;
    }
    private int _batchDepth;
    private readonly Dictionary<string, int> _batched = new(StringComparer.OrdinalIgnoreCase);

    private CancellationTokenSource? _debounceCts;
    private static readonly TimeSpan _workshopIdleFlush = TimeSpan.FromSeconds(2); // adjust as needed

    private bool _workshopBatchActive;
    private int _renderKey;

    public void NotifyAugmentationActivity()
    {
        // Start workshop batch only once
        if (_workshopBatchActive == false)
        {
            _workshopBatchActive = true;
            _batchDepth++; // begin (but only once for the whole session)
        }

        // Restart idle timer
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        _ = FlushAfterIdleAsync(token);
    }

    private async Task FlushAfterIdleAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(_workshopIdleFlush, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        // Only flush the workshop batch we started
        if (_workshopBatchActive)
        {
            _workshopBatchActive = false;
            End(); // this will drop depth to 0 and flush if _batched has anything
        }
    }
    public void Begin()
    {
        _batchDepth++;
    }

    public void End()
    {
        if (_batchDepth <= 0)
        {
            return;
        }

        _batchDepth--;
        if (_batchDepth > 0)
        {
            return;
        }

        if (_batched.Count == 0)
        {
            return;
        }

        var parts = _batched
            .OrderByDescending(x => x.Value)
            .Take(3)
            .Select(x => $"+{x.Value} {x.Key.GetWords}")
            .ToList();

        int remaining = _batched.Count - parts.Count;
        if (remaining > 0)
        {
            parts.Add($"+{remaining} more");
        }

        Toast!.ShowSuccessToast("Bonus received: " + string.Join(", ", parts));
        _batched.Clear();
    }
    public void Dispose()
    {
        if (farm.Current!.RulesManager.AutomationEnabled)
        {
            return;
        }
        farm.Current!.AnimalManager.OnAugmentedOutput -= OnAugmentedOutput;
        farm.Current.CropManager.OnAugmentedOutput -= OnAugmentedOutput;
        farm.Current.TreeManager.OnAugmentedOutput -= OnAugmentedOutput;
        farm.Current.WorkshopManager.OnAugmentedOutput -= OnAugmentedOutput;
    }
    

    //only one at a time can be used.
    //private 

    private void OnAugmentedOutput(ItemAmount obj)
    {
        if (obj.Amount <= 0)
        {
            return;
        }

        if (_batchDepth > 0)
        {
            _batched[obj.Item] = _batched.TryGetValue(obj.Item, out var old)
                ? old + obj.Amount
                : obj.Amount;
            return;
        }

        // not batching => immediate toast
        Toast?.ShowSuccessToast($"Bonus received: +{obj.Amount} {obj.Item.GetWords}");
    }


    public async Task OpenQuestOrScenarioBookAsync()
    {
        await CloseAllAsync();
        ShowQuestOrScenarioBook = true;
        Changed?.Invoke();
    }
    public void CloseQuestOrScenarioBook()
    {
        ShowQuestOrScenarioBook = false;
        Changed?.Invoke();
    }
    public void SetWorkshopVisible(bool visible)
    {
        if (visible == false)
        {
            CurrentWorkshop = null; //i think.
            CurrentAutomatedWorkshop = null; //i think this too now.
            Changed?.Invoke();
        }    
    }
    public void SetLevelsVisible(bool visible)
    {
        ShowLevelDetails = visible;
        Changed?.Invoke();
    }
    public async Task OpenLevelsAsync()
    {
        if (farm.CanCloseWorksiteAutomatically(CurrentWorksite) == false)
        {
            Toast?.ShowUserErrorToast("Must collect from worksite first before checking level progress");
            return;
        }
        await CloseAllAsync();
        ShowLevelDetails = true;
        Changed?.Invoke();
    }
    public void SetWorksiteVisible(bool visible)
    {
        if (visible == false)
        {
            CurrentWorksite = null;
            Changed?.Invoke();
        }
    }
    public string? CurrentWorksite { get; private set; }
    public WorkshopView? CurrentWorkshop { get; private set; }
    public WorkshopAutomationView? CurrentAutomatedWorkshop { get; private set; }
    public int RenderKey => _renderKey;
    public async Task OpenWorkshopAsync(WorkshopView workshop)
    {
        await CloseAllAsync();
        CurrentWorkshop = workshop;
        Changed?.Invoke(); //needs this.  otherwise, won't load.
    }

    public async Task OpenWorkshopAsync(WorkshopAutomationView workshop)
    {
        await CloseAllAsync();
        CurrentAutomatedWorkshop = workshop;
        Changed?.Invoke(); //needs this.  otherwise, won't load.
    }

    public async Task OpenPossibleWorksiteAsync(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return;
        }
        await CloseAllAsync();
        if (farm.Current!.RulesManager.AutomationEnabled == false)
        {
            if (farm.Current!.WorksiteManager.CanCollectRewards(location) && farm.Current.WorksiteManager.CanCollectRewardsWithLimits(location) == false)
            {
                Toast!.ShowUserErrorToast("Unable to open the worksite because your barn or silo is full");
                return;
            }
        }
        CurrentWorksite = location;
        Changed?.Invoke();
    }
    
    public async Task OpenAnimalsAsync()
    {
        await CloseAllAsync();
        ShowAnimals = true;
        Changed?.Invoke();
    }
    public async Task OpenCropsAsync()
    {
        await CloseAllAsync();
        ShowCrops = true;
        Changed?.Invoke();
    }
    public async Task OpenTreesAsync()
    {
        await CloseAllAsync();
        ShowTrees = true;
        Changed?.Invoke();
    }
    public void SetCropsVisible(bool visible)
    {
        ShowCrops = visible;
        Changed?.Invoke();
    }
    public void SetTreesVisible(bool visible)
    {
        ShowTrees = visible;
        Changed?.Invoke();
    }
    public void SetAnimalsVisible(bool visible)
    {
        ShowAnimals = visible;
        Changed?.Invoke();
    }
    public void Reset()
    {
        // Force the UI subtree to be recreated even if it's the same workshop again
        _renderKey++;
        ShowQuestOrScenarioBook = false;
        ShowTrees = false;
        ShowCrops = false;
        CurrentWorksite = null;
        ShowAnimals = false;
        CurrentWorkshop = null;
        CurrentAutomatedWorkshop = null;
    }
    public async Task CloseAllAsync()
    {
        if (farm.CanCloseWorksiteAutomatically(CurrentWorksite) == false)
        {
            Toast?.ShowUserErrorToast("Must collect from worksite first before closing all popups");
            return;
        }
        //await Task.Delay(1);
        Reset();
        await popup.CloseAllAsync(); //just in case.
        //
        Changed?.Invoke();
    }
}