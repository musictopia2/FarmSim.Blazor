namespace FarmSim.Blazor.Components.Crops;
public partial class AutomationCropsComponent
{
    private BasicList<CropAutomationView> _crops = [];
    private CropAutomationView? _crop;
    protected override void OnInitialized()
    {
        _crops = CropAutomationManager.GetUnlockedCrops;
    }
    private void OnCLick(CropAutomationView crop)
    {
        _crop = crop;
    }
    private void Previous()
    {
        if (_crop is null)
        {
            return;
        }
        int index = _crops.IndexOf(_crop);
        if (index == 0)
        {
            index = _crops.Count - 1;
        }
        else
        {
            index--;
        }
        _crop = _crops[index];
    }
    private void Next()
    {
        if (_crop is null)
        {
            return;
        }
        int index = _crops.IndexOf(_crop);
        if (index == _crops.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        _crop = _crops[index];
    }
}