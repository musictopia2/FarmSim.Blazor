namespace FarmSim.Blazor.Components.Worksites;
public partial class PowerGloveWorksiteComponent
{
    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    [EditorRequired]
    public string Location { get; set; }


    private bool _hasGloves;
    protected override void OnInitialized()
    {
        _hasGloves = InventoryManager.Has(CurrencyKeys.PowerGloveWorksite, 1);
        base.OnInitialized();
    }
    private int _used = 1;
    private void Increase()
    {
        _used++;
        if (InventoryManager.Has(CurrencyKeys.PowerGloveWorksite, _used) == false)
        {
            _used--; //cannot increase anymore because you don't have it.
        }
    }
    private void Decrease()
    {
        if (_used == 1)
        {
            return;
        }
        _used--;
    }
    private string TimeDecreased
    {
        get
        {
            TimeSpan original = PowerGloveRegistry.ReduceBy;
            original = original * _used;
            return original.GetTimeString;
        }
    }
    private void Submit()
    {
        if (RulesManager.AutomationEnabled == false)
        {
            WorksiteManager.UsePowerGlove(Location, _used);
        }
        else
        {
            WorksiteAutomationManager.UsePowerGlove(Location, _used);
        }
        OnClose.InvokeAsync();
    }
}