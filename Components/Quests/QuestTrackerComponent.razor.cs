namespace FarmSim.Blazor.Components.Quests;
public partial class QuestTrackerComponent(OverlayService questService, IToast toast, FarmTransferService transfer)
{
    private BasicList<QuestInstanceModel> _incompleteQuests = [];
    private const string _worksiteLockMessage = "Must collect from current worksite first";
    protected override Task OnInitializedAsync()
    {
        questService.Changed += Refresh;
        return base.OnInitializedAsync();
    }
    protected override void DisposeCore()
    {
        questService.Changed -= Refresh;
        base.DisposeCore();
    }
    private void Refresh()
    {
        LoadQuests();
        InvokeAsync(StateHasChanged);
    }

    private bool BlockIfLocked()
    {
        if (this.CanCloseWorksiteAutomatically(questService.CurrentWorksite))
        {
            return false;
        }
        toast.ShowUserErrorToast(_worksiteLockMessage);
        return true;

    }
    private async Task ShowAllQuestsAsync()
    {
        if (BlockIfLocked())
        {
            return;
        }
        await questService.OpenQuestOrScenarioBookAsync();
    }

    private static string NameStyle => "font-size:0.95rem;";

    private void LoadQuests()
    {
        _incompleteQuests = Farm!.QuestManager.ShowCurrentQuests(4);
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
    private bool CanCompleteQuest(QuestInstanceModel quest) => Farm!.QuestManager.CanCompleteQuest(quest);
    private async Task AttemptNavigationAsync(QuestInstanceModel quest)
    {
        if (RulesManager.AutomationEnabled == false)
        {
            WorkshopView? workshop = WorkshopManager.SearchForWorkshop(quest.ItemName);

            if (workshop is not null)
            {
                await questService.OpenWorkshopAsync(workshop);
                return;
            }
            if (TreeManager.HasTrees(quest.ItemName))
            {
                await questService.OpenTreesAsync();
                return;
            }
            if (CropManager.HasCrop(quest.ItemName))
            {
                await questService.OpenCropsAsync();
                return;
            }
            if (AnimalManager.HasAnimal(quest.ItemName))
            {
                await questService.OpenAnimalsAsync();
                return;
            }
            await questService.OpenPossibleWorksiteAsync(WorksiteManager.GetPossibleWorksiteForItem(quest.ItemName));
        }
        else
        {
            WorkshopAutomationView? workshop = WorkshopAutomationManager.SearchForWorkshop(quest.ItemName);
            if (workshop is not null)
            {
                await questService.OpenWorkshopAsync(workshop);
                return;
            }
            if (TreeAutomationManager.HasTrees(quest.ItemName))
            {
                await questService.OpenTreesAsync();
                return;
            }
            if (CropAutomationManager.HasCrops(quest.ItemName))
            {
                await questService.OpenCropsAsync();
                return;
            }
            if (AnimalAutomationManager.HasAnimal(quest.ItemName))
            {
                await questService.OpenAnimalsAsync();
                return;
            }
            await questService.OpenPossibleWorksiteAsync(WorksiteAutomationManager.GetPossibleWorksiteForItem(quest.ItemName));
        }
    }
    private async Task CompleteQuestAsync(QuestInstanceModel quest)
    {
        if (BlockIfLocked())
        {
            return;
        }
        if (CanCompleteQuest(quest) == false)
        {
            await AttemptNavigationAsync(quest);
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

    private int InventoryAmount(string itemKey)
    {
        // Whatever your inventory lookup is.
        // Example placeholder:
        return InventoryManager.Get(itemKey);
    }


}