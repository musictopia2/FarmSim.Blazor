namespace FarmSim.Blazor.Components.Main;
public partial class OverlayHost(OverlayService overlay) : IDisposable
{
    private string _flexibleHeader => Key.IsBaseline ? "Orders" : "Tasks";
    public void Dispose()
    {
        overlay.Changed -= Refresh;
        overlay.Dispose();

        GC.SuppressFinalize(this);
    }
    private void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }
    protected override void OnInitialized()
    {
        overlay.Changed += Refresh;
        overlay.Init();
        base.OnInitialized();
    }

    
}