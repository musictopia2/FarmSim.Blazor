namespace FarmSim.Blazor.Components.SpeedSeeds;
public partial class AnimalSpeedComponent(IToast toast)
{
    private BasicList<AnimalGrantModel> _animals = [];
    protected override void OnInitialized()
    {
        if (RulesManager.AutomationEnabled == false)
        {
            _animals = AnimalManager.GetUnlockedAnimalGrantItems();
        }
        else
        {
            _animals = AnimalAutomationManager.GetUnlockedAnimalGrantItems();
        }
        base.OnInitialized();
    }
    // how many of this animal type the player currently owns (unlocked instances)

    private string GetDisplay(AnimalGrantModel item)
    {
        //@AnimalManager.GetDisplayedGrantAmount(item) @item.OutputData.Item.GetWords
        if (RulesManager.AutomationEnabled == false)
        {
            return $"{AnimalManager.GetDisplayedGrantAmount(item)} {item.OutputData.Item.GetWords}";
        }
        return $"{AnimalAutomationManager.GetDisplayedGrantAmount(item)} {item.OutputData.Item.GetWords}";
    }

    private void TryToUse(AnimalGrantModel item)
    {
        if (RulesManager.AutomationEnabled == false)
        {
            if (AnimalManager.CanGrantAnimalItems(item, 1) == false)
            {
                toast.ShowUserErrorToast("Unable to use the speed seed.   Either don't have the requirements or the barn is full");
                return;
            }
            AnimalManager.GrantAnimalItems(item, 1);
            return;
        }
        if (AnimalAutomationManager.CanGrantAnimalItems(item, 1) == false)
        {
            toast.ShowUserErrorToast("Unable to use the speed seed.   Either don't have the requirements or the barn is full");
            return;
        }
        AnimalAutomationManager.GrantAnimalItems(item, 1);
    }
}