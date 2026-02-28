using BasicBlazorLibrary.Components.Layouts;
using BasicBlazorLibrary.Components.NavigationMenus;

namespace FarmSim.Blazor.Components.Main;

public partial class MainComponent(NavigationManager nav, OverlayService service,
    IToast toast, IntegerPickerService quantityPickerService,
    IntegerActionPickerService actionPickerService,
    FarmTransferService farmTransfer,
    INavigation custom,
    IPlayerNamePresenter playerNamePresenter
    ) : IDisposable
{

    private void Return()
    {
        FarmKey main = this.AsMain;
        nav.NavigateTo(main);
    }

    private NavigationBarContainer? _nav;

    private readonly OverlayInsets _overlays = new();
    private void VisibleChanged(bool visible)
    {
        quantityPickerService.UpdateVisibleStatus(visible);
    }
    private bool HasAnyActiveTimedBoosts => TimedBoostManager.GetActiveBoosts.Count > 0;
    private async Task OnValueChanged(int value)
    {
        quantityPickerService.Submit(value);

        await Task.CompletedTask;
    }

    private string ShowAllWorksitesConfirmation =>
        $"Are you sure you want to complete all worksites?  You have {InventoryManager.Get(CurrencyKeys.FinishAllWorksites)} left";

    private string ShowAllWorkshopsConfirmation =>
        $"Are you sure you want to complete all workshops?  You have {InventoryManager.Get(CurrencyKeys.FinishAllWorkshops)} left";


    private bool _showAllWorksites = false;
    private bool _showAllWorkshops = false;
    private bool _showRandomChestRewards = false;
    private bool _showAchievements = false;
    private bool _showAdvancedSettings = false;
    private void ShowAdvancedSettings()
    {
        _showAdvancedSettings = true;
    }
    private void ShowAchievements()
    {
        _showAchievements = true;
    }

    private void ShowRandomChestRewards()
    {
        if (InventoryManager.Has(CurrencyKeys.Chest, 1) == false)
        {
            toast.ShowUserErrorToast("You have no chests to open");
            return;
        }
        _showRandomChestRewards = true;
    }
    private async Task TransferCoinsAsync()
    {
        await ScenarioManager.ClaimRewardAsync(custom, Key, farmTransfer, toast);
    }
    private void ProcessAllWorksites(bool confirmed)
    {
        _showAllWorksites = false;
        if (confirmed)
        {
            if (RulesManager.AutomationEnabled == false)
            {
                WorksiteManager.CompleteAllJobsImmediately();
            }
            else
            {
                WorksiteAutomationManager.CompleteAllJobsImmediately();
            }
            toast.ShowSuccessToast("Completed all jobs from the worksites.  If there was none that was active, went to waste");
        }
    }
    private void ProcessAllWorkshops(bool confirmed)
    {
        _showAllWorkshops = false;
        if (confirmed)
        {
            if (RulesManager.AutomationEnabled == false)
            {
                WorkshopManager.CompleteAllJobsImmediately();
            }
            else
            {
                WorkshopAutomationManager.CompleteAllJobsImmediately();
            }
            toast.ShowSuccessToast("Completed all jobs from the workshops.  If there was none that was active, went to waste");
        }
    }

    private void PossibleAllWorksites()
    {
        if (InventoryManager.Has(CurrencyKeys.FinishAllWorksites, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have any Finish All Worksites consumables left.  Try to purchase some");
            return;
        }
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showAllWorksites = true;
        }
        else
        {
            ProcessAllWorksites(true);
        }
    }
    private void PossibleAllWorkshops()
    {
        if (InventoryManager.Has(CurrencyKeys.FinishAllWorkshops, 1) == false)
        {
            toast.ShowUserErrorToast("You do not have any Finish All Workshops consumables left.  Try to purchase some");
            return;
        }
        if (AdvancedSettingsManager.Settings.UseConfirmations)
        {
            _showAllWorkshops = true;
        }
        else
        {
            ProcessAllWorkshops(true);
        }
    }


    private void ShowActiveBoosts()
    {
        _showActiveBoosts = true;
    }

    private void Changed()
    {
        InvokeAsync(StateHasChanged);
    }
    protected override void OnAfterRender(bool firstRender)
    {
        if (_nav is not null)
        {
            _overlays.TopPx = _nav.HeightOfHeader + 5;
            _overlays.BottomPx = 10;
            StateHasChanged();
            return;
        }
        base.OnAfterRender(firstRender);
    }

    private bool _loaded = false;
    protected override void OnInitialized()
    {
        if (Key.IsCoin == false)
        {
            _loaded = true;
            //toast.ShowInfoToast("Coin.  Rethink");
        }
        quantityPickerService.StateChanged = Changed;
        TimedBoostManager.Tick += Changed;
        ScenarioManager.OnUpdated += TryUpdateTasks;
        AchievementManager.OnAchievementSuccessful += AchievementManager_OnAchievementSuccessful;
        service.Toast = toast;
        base.OnInitialized();
    }

    private void AchievementManager_OnAchievementSuccessful(string title, int reward)
    {
        toast.ShowSuccessToast($"You completed at least one achievement for {title}.   Coin reward was {reward}");
    }

    private void TryUpdateTasks()
    {
        _status = ScenarioManager.GetStatus;
        if (_status == EnumScenarioStatus.Cooldown)
        {
            _nextScenarioStatus = ScenarioManager.TimeLeft();
        }
    }

    private EnumScenarioStatus _status;
    private string _nextScenarioStatus = "";
    protected override async Task OnInitializedAsync()
    {
        await service.CloseAllAsync();
        if (Key.IsCoin)
        {
            await ScenarioManager.LoadAsync();
            TryUpdateTasks();
            _loaded = true;
        }
        await base.OnInitializedAsync();
    }

    private void Travel()
    {
        FarmKey coop = this.AsCooperative;
        nav.NavigateTo(coop);
    }

    private void Home()
    {
        if (this.IsCoin || this.IsCooperative)
        {
            FarmKey main = this.AsMain;
            nav.NavigateTo(main);
            return;
        }
        //has to figure out an easy way eventually to enable the coop (later).
        service.Reset();
        nav.NavigateTo("/");
    }
    private bool _showBarn = false;
    private bool _showSilo = false;
    private bool _showShop = false;
    private bool _showSpeedSeeds = false;
    private bool _showTimedBoosts = false;
    private bool _showActiveBoosts = false;

    private async Task CloseAllPopupsAsync()
    {
        await service.CloseAllAsync();
    }
    private void ShowTimedBoosts()
    {
        _showTimedBoosts = true;
    }
    private void ShowSpeedSeeds()
    {
        _showSpeedSeeds = true;
    }
    private void ShowShop()
    {
        _showShop = true;
    }
    private void ShowSilo()
    {
        _showSilo = true;
    }

    private void ShowBarn()
    {
        _showBarn = true;
    }
    private void CloseBarn()
    {
        _showBarn = false;
    }

    public void Dispose()
    {
        quantityPickerService.StateChanged = null;
        TimedBoostManager.Tick -= Changed;
        ScenarioManager.OnUpdated -= TryUpdateTasks;
        GC.SuppressFinalize(this);
    }

    private string Title
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Key.Theme))
            {
                return "Needs Theme";
            }
            return $"{ playerNamePresenter.ToDisplayName(Key.PlayerName)} {Key.Theme.GetWords} {Key.ProfileId.GetWords}";
        }
    }
    private void ActionVisibleChanged(bool visible)
    {
        actionPickerService.UpdateVisibleStatus(visible);
    }

    private async Task OnActionValueChanged(int value)
    {
        await actionPickerService.SubmitAsync(value);
    }

}