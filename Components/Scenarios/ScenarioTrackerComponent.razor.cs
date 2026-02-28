namespace FarmSim.Blazor.Components.Scenarios;
public partial class ScenarioTrackerComponent(OverlayService overlayService, IToast toast)
{
    private BasicList<ScenarioInstance> _incompleteTasks = [];
    private const string _worksiteLockMessage = "Must collect from current worksite first";
    protected override Task OnInitializedAsync()
    {
        overlayService.Changed += Refresh;
        ScenarioManager.OnUpdated += Refresh;
        return base.OnInitializedAsync();
    }
    protected override void DisposeCore()
    {
        overlayService.Changed -= Refresh;
        ScenarioManager.OnUpdated -= Refresh;
        base.DisposeCore();
    }
    private void Refresh()
    {
        LoadScenarios();
        InvokeAsync(StateHasChanged);
    }

    private bool BlockIfLocked()
    {
        if (this.CanCloseWorksiteAutomatically(overlayService.CurrentWorksite))
        {
            return false;
        }
        toast.ShowUserErrorToast(_worksiteLockMessage);
        return true;

    }
    private async Task ShowAllTasksAsync()
    {
        if (BlockIfLocked())
        {
            return;
        }
        await overlayService.OpenQuestOrScenarioBookAsync();
    }
    private void LoadScenarios()
    {
        _incompleteTasks = Farm!.ScenarioManager.ShowCurrentScenarios(4);
    }
    protected override void OnInitialized()
    {
        LoadScenarios();
        base.OnInitialized();
    }
    protected override Task OnInventoryChangedAsync()
    {
        LoadScenarios();
        return base.OnInventoryChangedAsync();
    }
    private bool CanCompleteScenario(ScenarioInstance task) => Farm!.ScenarioManager.CanCompleteTask(task);
    private async Task AttemptNavigationAsync(ScenarioInstance task)
    {

        WorkshopView? workshop = WorkshopManager.SearchForWorkshop(task.Item);

        if (workshop is not null)
        {
            await overlayService.OpenWorkshopAsync(workshop);
            return;
        }

        //attempt animals.
        if (AnimalManager.HasAnimal(task.Item))
        {
            await overlayService.OpenAnimalsAsync();
            return;
        }
        if (CropManager.HasCrop(task.Item))
        {
            await overlayService.OpenCropsAsync();
            return;
        }
        if (TreeManager.HasTrees(task.Item))
        {
            await overlayService.OpenTreesAsync();
            return;
        }
        await overlayService.OpenPossibleWorksiteAsync(WorksiteManager.GetPossibleWorksiteForItem(task.Item));
    }
    private async Task CompleteTaskAsync(ScenarioInstance task)
    {
        if (BlockIfLocked())
        {
            return;
        }
        if (CanCompleteScenario(task) == false)
        {
            await AttemptNavigationAsync(task);
            return;
        }

        await Farm!.ScenarioManager.CompleteTaskAsync(task);
        LoadScenarios();
    }

    private int InventoryAmount(string itemKey)
    {
        // Whatever your inventory lookup is.
        // Example placeholder:
        return InventoryManager.Get(itemKey);
    }

}