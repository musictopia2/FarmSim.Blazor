namespace FarmSim.Blazor.Components.Animals;
public partial class AutomationAnimalComponent(OverlayService overlay, IToast toast)
{
    [Parameter]
    [EditorRequired]
    public AnimalAutomationView Animal { get; set; }
    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }

    AnimalProductionOption? _option;
    private int _has;
    private bool _showPicker;
    private int _pickerLimit;
    private string _pickerVerb = "";
    private int _cap;
    private int _outstanding;
    private int _remaining;
    private bool _blocked;
    private string _nextReadyDisplay = "";
    private int _virtualCount;
    private async void RefreshComputedState()
    {
        if (_option is null)
        {
            return;
        }
        if (AnimalAutomationManager.IsSuppressed(Animal.AnimalName))
        {
            toast.ShowSuccessToast($"Successfully converted {Animal.AnimalName} to instant unlimited.  Therefore, closing out of this animal");
            await overlay.CloseAllAsync();
            return;
        }
        _virtualCount = AnimalAutomationManager.GetVirtualCount(Animal.AnimalName);
        _has = InventoryManager.Get(_option.Required);
        _cap = AnimalAutomationManager.GetCap(Animal.AnimalName);
        _outstanding = AnimalAutomationManager.GetOutstanding(Animal.AnimalName);
        _remaining = AnimalAutomationManager.GetRemainingCapacity(Animal.AnimalName);
        _blocked = AnimalAutomationManager.IsBlocked(Animal);
        var next = AnimalAutomationManager.GetTimeUntilNextReady(Animal.AnimalName);
        _nextReadyDisplay =
            _blocked ? "Blocked" :
            next is null ? "—" :
            next.Value == TimeSpan.Zero ? "Ready now" :
            next.Value.GetTimeString;
    }
    protected override void OnParametersSet()
    {
        _option = AnimalAutomationManager.GetSelectedOption(Animal.AnimalName);
        int count = AnimalAutomationManager.VirtualCount(Animal.AnimalName);
        _option = new()
        {
            Duration = _option.Duration,
            Input = _option.Input * count,
            Output = _option.Output,
            Required = _option.Required
        };
        RefreshComputedState();
        base.OnParametersSet();
    }

    private async Task ClickedCropAsync(string name)
    {
        if (name == "")
        {
            return;
        }
        //just clicking means needs to open the crops.
        await overlay.OpenCropsAsync();
    }


    protected override Task OnTickAsync()
    {
        RefreshComputedState();
        return base.OnTickAsync();
    }

    private void OpenRequestPicker()
    {
        //_pickerMode = EnumPickerMode.Request;
        _pickerVerb = "Request";
        _pickerLimit = _cap; //iffy.
        _showPicker = true;
    }

    private void OpenAddPicker()
    {
        //_pickerMode = EnumPickerMode.Add;
        _pickerVerb = "Add";
        _pickerLimit = _remaining;
        _showPicker = true;
    }

    private Task ClosePicker()
    {
        _showPicker = false;
        return Task.CompletedTask;
    }

    private Task SubmitPickerAsync(int amount)
    {
        _showPicker = false;

        // In your manager, this can just be the same method. “Add” is still “Request more”.
        AnimalAutomationManager.Request(Animal.AnimalName, amount);

        RefreshComputedState();
        return InvokeAsync(StateHasChanged);
    }


}