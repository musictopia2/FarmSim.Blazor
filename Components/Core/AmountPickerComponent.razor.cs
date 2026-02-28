namespace FarmSim.Blazor.Components.Core;

public partial class AmountPickerComponent(IntegerPickerService quantityPickerService) : IDisposable
{
    [Parameter, EditorRequired] public int Limit { get; set; }
    [Parameter, EditorRequired] public string ActionVerb { get; set; } = "";
    [Parameter] public EventCallback<int> OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private int _proposed;

    protected override void OnInitialized()
    {
        quantityPickerService.Completed = ChoseCustom;
        _proposed = Limit; //start out with limit since a person would usually want to go to the limit.
        base.OnInitialized();
    }

    private void ChoseCustom(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _proposed = amount > Limit ? Limit : amount;
        InvokeAsync(StateHasChanged);
    }

    private void OpenCustom() => quantityPickerService.Pick(Limit);

    private void Submit()
    {
        if (_proposed <= 0)
        {
            return;
        }

        OnSubmit.InvokeAsync(_proposed);
    }

    private void GoLeft()
    {
        if (_proposed <= 1)
        {
            return;
        }

        _proposed--;
    }

    private void GoRight()
    {
        if (_proposed >= Limit)
        {
            return;
        }

        _proposed++;
    }

    private void ProposeAll() => _proposed = Limit;
    private void ProposeOne() => _proposed = 1;

    public void Dispose()
    {
        quantityPickerService.Completed = null;
        GC.SuppressFinalize(this);
    }
}