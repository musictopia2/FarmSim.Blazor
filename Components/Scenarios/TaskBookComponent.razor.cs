namespace FarmSim.Blazor.Components.Scenarios;
public partial class TaskBookComponent
{
    private BasicList<ScenarioInstance> _incompleteTasks = [];
    private bool _markSeenAfterRender;
    private void LoadTask()
    {
        _incompleteTasks = ScenarioManager.GetAllIncompleteTasks();
        _markSeenAfterRender = _incompleteTasks.Any(x => x.Seen == false);
    }

    protected override void OnInitialized()
    {
        LoadTask();
        base.OnInitialized();
    }

    protected override Task OnInventoryChangedAsync()
    {
        LoadTask();
        return base.OnInventoryChangedAsync();
    }

    private async Task ToggleTrackedAsync(ScenarioInstance task)
    {
        await ScenarioManager.SetTrackedAsync(task, !task.Tracked, 4);
        LoadTask(); // refresh star states and any ordering you choose
    }
    private async Task CompleteTaskAsync(ScenarioInstance task)
    {
        if (ScenarioManager.CanCompleteTask(task) == false)
        {
            return;
        }
        await ScenarioManager.CompleteTaskAsync(task);
        LoadTask();
    }
    private int InventoryAmount(string itemKey) => InventoryManager.Get(itemKey);


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Mark "new" quests as seen AFTER they were shown once.
        if (_markSeenAfterRender)
        {
            _markSeenAfterRender = false;
            await ScenarioManager.MarkAllIncompleteSeenAsync(); //obviously don't rerender this time.
        }
    }
}