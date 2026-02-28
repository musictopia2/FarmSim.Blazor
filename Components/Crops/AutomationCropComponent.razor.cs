namespace FarmSim.Blazor.Components.Crops;
public partial class AutomationCropComponent(OverlayService overlay, IToast toast)
{
    [Parameter, EditorRequired]
    public CropAutomationView Crop { get; set; }


    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }


    private bool _showPicker;
    private int _pickerLimit;
    private string _pickerVerb = "";
    private int _cap;
    private int _virtualCount;
    private int _outstanding;
    private int _remaining;
    private bool _blocked;
    private string _nextReadyDisplay = "";
    protected override void OnParametersSet()
    {
        RefreshComputedState();
        base.OnParametersSet();
    }
    protected override Task OnTickAsync()
    {
        RefreshComputedState();
        return base.OnTickAsync();
    }
    private async void RefreshComputedState()
    {
        if (CropAutomationManager.IsSuppressed(Crop.CropName))
        {
            toast.ShowSuccessToast($"Successfully converted {Crop.CropName} to instant unlimited.  Therefore, closing out of this crop");
            await overlay.CloseAllAsync();
            return;
        }
        _cap = CropAutomationManager.GetCap(Crop.CropName);
        _virtualCount = CropAutomationManager.GetVirtualCount(Crop.CropName);
        _outstanding = CropAutomationManager.GetOutstanding(Crop.CropName);
        _remaining = CropAutomationManager.GetRemainingCapacity(Crop.CropName);
        _blocked = CropAutomationManager.IsBlocked(Crop);

        var next = CropAutomationManager.GetTimeUntilNextReady(Crop.CropName);
        _nextReadyDisplay =
            _blocked ? "Blocked" :
            next is null ? "—" :
            next.Value == TimeSpan.Zero ? "Ready now" :
            next.Value.GetTimeString;
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
        CropAutomationManager.Request(Crop.CropName, amount);

        RefreshComputedState();
        return InvokeAsync(StateHasChanged);
    }
}