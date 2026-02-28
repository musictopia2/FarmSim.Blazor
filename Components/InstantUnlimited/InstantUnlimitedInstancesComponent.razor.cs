namespace FarmSim.Blazor.Components.InstantUnlimited;
public partial class InstantUnlimitedInstancesComponent(IToast toast) : IDisposable
{

    private BasicList<string> _instances = [];

    private bool _openOptions = false;
    private string _currentOption = "";
    private string _timeLeft = "";
    protected override void OnInitialized()
    {
        LoadList();
        Farm!.InstantUnlimitedManager.Changed += Refresh;

        base.OnInitialized();
    }
    private void LoadList()
    {
        _instances = Farm!.InstantUnlimitedManager.UnlockedInstances;
        if (_openOptions && Farm!.InstantUnlimitedManager.IsUnlocked(_currentOption) == false)
        {
            _openOptions = false;
            _currentOption = "";
            return;
        }
    }
    private void Refresh()
    {
        LoadList();
        InvokeAsync(StateHasChanged);
    }


    //i don't think this is quite a modal.

    private void ChooseInstance(string instance)
    {
        _currentOption = instance;
        _openOptions = true;
        _timeLeft = RentalManager.GetDurationString(instance);
    }

    private void OnChose(int howMany)
    {
        if (Farm!.InstantUnlimitedManager.IsUnlocked(_currentOption) == false)
        {
            toast.ShowUserErrorToast("Unable to add to inventory because this possible rental expired");
            _openOptions = false;
            _currentOption = "";
            return; //close out anyways.
        }
        if (Farm!.InstantUnlimitedManager.CanApplyInstantUnlimited(_currentOption, howMany) == false)
        {
            toast.ShowUserErrorToast("Unable to add to inventory.  Most likely out of barn or silo space");
            _openOptions = true;
            return;
        }
        Farm.InstantUnlimitedManager.ApplyInstantUnlimited(_currentOption, howMany);
        _currentOption = "";
        _openOptions = false;
    }

    private const string InfinitySvg =
        @"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'>
            <circle cx='32' cy='32' r='28' fill='rgba(0,0,0,0.70)'/>
            <path d='M18 34c3-8 11-8 16 0s13 8 16 0'
                  fill='none' stroke='white' stroke-width='6' stroke-linecap='round'/>
            <path d='M18 34c3 8 11 8 16 0s13-8 16 0'
                  fill='none' stroke='white' stroke-width='6' stroke-linecap='round'/>
        </svg>";

    public void Dispose()
    {
        Farm!.InstantUnlimitedManager.Changed -= Refresh;
        GC.SuppressFinalize(this);
    }
}