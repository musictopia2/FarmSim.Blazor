namespace FarmSim.Blazor.Components.Trees;
public partial class AutomationTreeComponent(IToast toast, OverlayService overlay)
{
    [Parameter]
    [EditorRequired]
    public TreeAutomationView Tree { get; set; }
    [Parameter]
    public EventCallback NextClicked { get; set; }

    [Parameter]
    public EventCallback PreviousClicked { get; set; }
    private bool _showPicker;
    private int _pickerLimit;
    private string _pickerVerb = "";
    // store intent so Submit knows what to do if you ever split behaviors later
    //private EnumPickerMode _pickerMode = EnumPickerMode.Request;

    //private enum EnumPickerMode { Request, Add }

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
        if (TreeAutomationManager.IsSuppressed(Tree.TreeName))
        {
            toast.ShowSuccessToast($"Successfully converted {Tree.TreeName} to instant unlimited.  Therefore, closing out of this tree");
            await overlay.CloseAllAsync();
            return;
        }
        _cap = TreeAutomationManager.GetCap(Tree.TreeName);
        _virtualCount = TreeAutomationManager.GetVirtualCount(Tree.TreeName);
        _outstanding = TreeAutomationManager.GetOutstanding(Tree.TreeName);
        _remaining = TreeAutomationManager.GetRemainingCapacity(Tree.TreeName);
        _blocked = TreeAutomationManager.IsBlocked(Tree);

        var next = TreeAutomationManager.GetTimeUntilNextReady(Tree.TreeName);
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
        TreeAutomationManager.Request(Tree.TreeName, amount);

        RefreshComputedState();
        return InvokeAsync(StateHasChanged);
    }


}