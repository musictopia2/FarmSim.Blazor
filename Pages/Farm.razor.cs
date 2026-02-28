namespace FarmSim.Blazor.Pages;
public partial class Farm(GameRegistry registry,
    FarmContext context,
    IFarmRulesProvider rulesProvider,
    IGameTickRunner tickRunner) : IDisposable
{
    [Parameter]
    public string Theme { get; set; } = string.Empty;

    [Parameter]
    public string Player { get; set; } = string.Empty;

    [Parameter]
    public string ProfileId { get; set; } = string.Empty;
    [Parameter] public string SlotText { get; set; } = "";
    private EnumFarmSlot Slot { get; set; } = EnumFarmSlot.Main;

    private MainFarmContainer? _farmContainer;


    private bool _needsSetup = false;
    private bool _loaded = false;

    private FarmKey _farmKey;
    private bool _allowed = true;
    private bool _coinNotAllowed = false;
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrWhiteSpace(SlotText) &&
        Enum.TryParse<EnumFarmSlot>(SlotText, ignoreCase: true, out var parsed))
        {
            Slot = parsed;
        }
        else
        {
            Slot = EnumFarmSlot.Main;
        }
        FarmKey player = new()
        {
            Theme = Theme,
            PlayerName = Player,
            ProfileId = ProfileId,
            Slot = Slot
        };
        EnumRuleCategory category = await rulesProvider.GetRulesAsync(Player);
        if (category == EnumRuleCategory.NotChosen)
        {
            _needsSetup = true;
            _farmKey = player;
        }
        else if (category == EnumRuleCategory.Automated && player.IsCooperative)
        {
            _allowed = false;
        }
        else
        {
            _farmContainer = registry.GetFarm(player);
            if (_farmContainer.RulesManager.RuleChosen == EnumRuleCategory.Automated && player.IsCoin)
            {
                _coinNotAllowed = true;
            }
            context.Set(_farmContainer);
        }
        _loaded = true;
        await base.OnParametersSetAsync();
    }
    protected override void OnInitialized()
    {
        BlazoredToasts.Timeout = 2; //needs to be less time.
        BlazoredToasts.TopOffset = "250px";
        base.OnInitialized();
    }
    private async Task SaveRulesAndInitializeAsync(EnumRuleCategory category)
    {
        CustomBasicException.ThrowIfNull(_farmKey);
        await rulesProvider.SaveRulesAsync(_farmKey.PlayerName, category);
        await tickRunner.InitializeFarmsAsync(_farmKey.PlayerName);
        if (category == EnumRuleCategory.Automated && _farmKey.IsCooperative)
        {
            _allowed = false;
        }
        else
        {
            _farmContainer = registry.GetFarm(_farmKey);
            if (_farmContainer.RulesManager.RuleChosen == EnumRuleCategory.Automated && _farmKey.IsCoin)
            {
                _coinNotAllowed = true;
            }
            _needsSetup = false;
            context.Set(_farmContainer);
            StateHasChanged();
        }
    }
    public void Dispose()
    {
        BlazoredToasts.TopOffset = "4rem"; //the default.
        GC.SuppressFinalize(this);
    }
}