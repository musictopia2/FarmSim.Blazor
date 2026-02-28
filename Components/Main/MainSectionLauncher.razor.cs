namespace FarmSim.Blazor.Components.Main;

public partial class MainSectionLauncher(ReadyStatusService readyService,
    OverlayService overlay
    ) : IDisposable
{
    [Parameter]
    [EditorRequired]
    public EnumMainSection ReadyCategory { get; set; }
    private void ChangeWorkshopVisible(bool visible)
    {
        if (ReadyCategory != EnumMainSection.Workshops)
        {
            return; //should only work for workshops.
        }
        _show = visible;
    }

    protected override void OnInitialized()
    {
        readyService.OnChanged += HandleReadyChanged;
        UpdateStatus();
        base.OnInitialized();
    }

    public void Dispose()
    {
        readyService.OnChanged -= HandleReadyChanged;
        GC.SuppressFinalize(this);
    }
    private bool _ready;

    private bool _show;

    private async Task LaunchAsync()
    {
        if (ReadyCategory == EnumMainSection.Workshops)
        {

            _show = true;
            return;
        }
        if (ReadyCategory == EnumMainSection.Trees)
        {
            await overlay.OpenTreesAsync();
            return;
        }
        if (ReadyCategory == EnumMainSection.Crops)
        {
            await overlay.OpenCropsAsync();
            return;
        }
        if (ReadyCategory == EnumMainSection.Animals)
        {
            await overlay.OpenAnimalsAsync();
            return;
        }
    }

    private void UpdateStatus()
    {
        if (ReadyCategory == EnumMainSection.Trees)
        {
            _ready = readyService.Current.Trees;
        }
        if (ReadyCategory == EnumMainSection.Crops)
        {
            _ready = readyService.Current.Crops;
        }
        if (ReadyCategory == EnumMainSection.Animals)
        {
            _ready = readyService.Current.Animals;
        }
        if (ReadyCategory == EnumMainSection.Workshops)
        {
            _ready = readyService.Current.Workshops;
        }
    }

    private void HandleReadyChanged()
    {
        UpdateStatus();

        InvokeAsync(StateHasChanged);
    }

}