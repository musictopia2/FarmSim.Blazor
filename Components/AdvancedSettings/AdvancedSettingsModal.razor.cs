namespace FarmSim.Blazor.Components.AdvancedSettings;
public partial class AdvancedSettingsModal(GameRegistry registry)
{
    private AdvancedSettingsProfileModel Settings { get; set; } = new();
    private bool _saved = false;
    private DateTime _lastdate;
    private BasicList<string> _workers = [];
    protected override void OnInitialized()
    {
        Settings = AdvancedSettingsManager.Settings;
        if (RulesManager.AutomationEnabled == false)
        {
            _workers = WorksiteManager.GetAllUnlockedWorkers();
        }
        base.OnInitialized();
    }
    private async Task WorkshopPolicyChanged(EnumWorkshopCollectionCategories policy)
    {
        await SaveAsync();
    }
    private async Task ProcessWorkerAsync()
    {
        CustomBasicException.ThrowIfNull(_currentWorker);
        if (_preferredList.Any(x => x == _currentWorker.WorkerName))
        {
            _preferredList.RemoveSpecificItem(_currentWorker.WorkerName);
        }
        else
        {
            _preferredList.Add(_currentWorker.WorkerName);
        }
        Settings.SetPreferredWorkersForWorksite(_selectedSite, _preferredList);
        await SaveAsync();
    }

    private BasicList<string> _preferredList = [];
    private async Task ClearAllAsync()
    {
        _preferredList.Clear();
        Settings.SetPreferredWorkersForWorksite(_selectedSite, _preferredList);
        await SaveAsync();
    }
    private string GetDetails(string name) => WorksiteManager.GetWorkerDetails(name).Details;
    private int _workerIndex = 0;
    private bool CanGoUp => _workerIndex > 0;
    private bool CanGoDown => _workerIndex < _workers.Count - 1;
    private WorkerRecipe? _currentWorker;
    private void GoUp()
    {
        _workerIndex--;
        PopulateCurrentWorker();
    }

    private void PopulateCurrentWorker()
    {
        string name = _workers[_workerIndex];
        _currentWorker = WorksiteManager.GetWorkerDetails(name);
    }
    private void GoDown()
    {
        _workerIndex++;
        PopulateCurrentWorker();
    }
    private string WorksiteImageUrl => $"/{_selectedSite}.png";
    private void SelectSite(string site)
    {
        _selectedSite = site;
        _workerIndex = 0;
        PopulateCurrentWorker();
        _preferredList = Settings.GetPreferredWorkersForWorksite(site);
    }
    private string _selectedSite = "";
    private void BackToList()
    {
        _selectedSite = "";
    }
    private int GetPreferredCount(string site)
        => Settings.GetPreferredWorkersForWorksite(site).Count;
    private async Task SaveAsync()
    {
        await AdvancedSettingsManager.SaveSettingsAsync(Key, Settings, registry);
        _lastdate = DateTime.Now;
        _saved = true;
    }
}