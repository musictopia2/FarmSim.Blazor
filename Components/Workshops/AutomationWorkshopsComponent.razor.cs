namespace FarmSim.Blazor.Components.Workshops;
public partial class AutomationWorkshopsComponent(OverlayService overlay)
{
    [Parameter]
    public WorkshopAutomationView? SpecificWorkshop { get; set; }
    private WorkshopAutomationView? _currentWorkshop;
    private BasicList<WorkshopAutomationView> _workshops = [];
    protected override void OnInitialized()
    {
        UpdateWorkshops();
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        if (SpecificWorkshop is not null)
        {
            _currentWorkshop = SpecificWorkshop;
        }
    }
    private void SelectWorkshop(WorkshopAutomationView workshop)
    {
        _currentWorkshop = workshop;
    }

    private void UpdateWorkshops()
    {
        _workshops = WorkshopAutomationManager.GetUnlockedWorkshops;

    }
    private async Task TryAnimalsAsync(string name)
    {
        if (AnimalAutomationManager.HasAnimal(name))
        {
            await overlay.OpenAnimalsAsync();
            return;
        }
        await TryCropsAsync(name);
        //can eventually try crops and even trees.
    }

    private async Task TryCropsAsync(string name)
    {
        if (CropAutomationManager.HasCrops(name))
        {
            await overlay.OpenCropsAsync();
            return;
        }
        await TryTreesAsync(name);
    }
    private async Task TryTreesAsync(string name)
    {
        if (TreeAutomationManager.HasTrees(name))
        {
            await overlay.OpenTreesAsync();
            return;
        }
        await TryWorksitesAsync(name);
    }
    private async Task TryWorksitesAsync(string name)
    {
        string? location;
        if (RulesManager.AutomationEnabled == false)
        {
            location = WorksiteManager.GetPossibleWorksiteForItem(name);
        }
        else
        {
            location = WorksiteAutomationManager.GetPossibleWorksiteForItem(name);
        }
        await overlay.OpenPossibleWorksiteAsync(location);
    }

    private async Task NavigateToAsync(string name)
    {
        var possible = WorkshopAutomationManager.SearchForWorkshop(name);
        if (possible is null)
        {
            await TryAnimalsAsync(name);
            return;
        }


        _currentWorkshop = possible;

        await InvokeAsync(StateHasChanged);
    }
    private void Previous()
    {
        if (_currentWorkshop is null)
        {
            return;
        }
        int index = _workshops.IndexOf(_currentWorkshop);
        if (index == 0)
        {
            index = _workshops.Count - 1;
        }
        else
        {
            index--;
        }
        _currentWorkshop = _workshops[index];
    }
    private void Next()
    {
        if (_currentWorkshop is null)
        {
            return;
        }
        int index = _workshops.IndexOf(_currentWorkshop);
        if (index == _workshops.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        _currentWorkshop = _workshops[index];
    }

}