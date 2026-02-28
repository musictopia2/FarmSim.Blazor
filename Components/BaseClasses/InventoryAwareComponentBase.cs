namespace FarmSim.Blazor.Components.BaseClasses;
public abstract class InventoryAwareComponentBase : FarmComponentBase, IDisposable
{
    protected override void OnInitialized()
    {
        InventoryManager.InventoryChanged += async () => await OnInventoryChangedAsync();
        base.OnInitialized();
    }
    protected virtual async Task OnInventoryChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }
    protected virtual void DisposeCore()
    {

    }
    public void Dispose()
    {
        InventoryManager?.InventoryChanged -= async () => await OnInventoryChangedAsync();
        DisposeCore();
        GC.SuppressFinalize(this);
    }
}