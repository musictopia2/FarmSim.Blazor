namespace FarmSim.Blazor.Components.Workshops;
public partial class ManualPowerGloveWorkshopComponent()
{
    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    [EditorRequired]
    public WorkshopView Workshop { get; set; }


    private bool _hasGloves;
    protected override void OnInitialized()
    {
        _hasGloves = InventoryManager.Has(CurrencyKeys.PowerGloveWorkshop, 1);
        base.OnInitialized();
    }

    private int _used = 1;


    private void Increase()
    {
        _used++;
        if (InventoryManager.Has(CurrencyKeys.PowerGloveWorkshop, _used) == false)
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
        WorkshopManager.UsePowerGlove(Workshop, _used);
        OnClose.InvokeAsync();
    }

}