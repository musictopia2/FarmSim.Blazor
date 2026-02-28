namespace FarmSim.Blazor.Components.Quests;

public partial class QuestBookComponent(IToast toast, FarmTransferService transfer)
{
    private BasicList<QuestInstanceModel> _incompleteQuests = [];
    private bool _markSeenAfterRender;
    private void LoadQuests()
    {
        _incompleteQuests = Farm!.QuestManager.GetAllIncompleteQuests();
        _markSeenAfterRender = _incompleteQuests.Any(x => x.Seen == false);
    }

    protected override void OnInitialized()
    {
        LoadQuests();
        base.OnInitialized();
    }

    protected override Task OnInventoryChangedAsync()
    {
        LoadQuests();
        return base.OnInventoryChangedAsync();
    }

    private async Task ToggleTrackedAsync(QuestInstanceModel quest)
    {
        await Farm!.QuestManager.SetTrackedAsync(quest, !quest.Tracked, 4);
        LoadQuests(); // refresh star states and any ordering you choose
    }
    private async Task CompleteQuestAsync(QuestInstanceModel quest)
    {
        if (Farm!.QuestManager.CanCompleteQuest(quest) == false)
        {
            return;
        }
        if (quest.LevelRequired > ProgressionManager.CurrentLevel)
        {
            toast.ShowUserErrorToast("Must be higher level to complete the quest");
            return;
        }
        await Farm!.QuestManager.CompleteQuestAsync(Key, transfer, quest);
        LoadQuests();
    }
    private int InventoryAmount(string itemKey) => InventoryManager.Get(itemKey);


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Mark "new" quests as seen AFTER they were shown once.
        if (_markSeenAfterRender)
        {
            _markSeenAfterRender = false;
            await Farm!.QuestManager.MarkAllIncompleteSeenAsync(); //obviously don't rerender this time.
        }
    }

}