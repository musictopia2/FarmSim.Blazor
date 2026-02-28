namespace FarmSim.Blazor.Components.RandomChests;
public partial class RandomChestRewardComponent(IMessageBox message)
{
    private int _openedCount;
    private bool _loaded = false;
    private BasicList<RandomChestResultModel> _rewards = [];
    protected override async Task OnInitializedAsync()
    {
        _openedCount = InventoryManager.Get(CurrencyKeys.Chest);
        _rewards = await Farm!.RandomChestManager.OpenChestsAsync();
        _loaded = true;
    }
    private async Task ShowAugmentationDetailsAsync(RandomChestResultModel reward)
    {
        if (string.IsNullOrWhiteSpace(reward.OutputAugmentationKey))
        {
            return;
        }

        string description = Farm!.OutputAugmentationManager.GetDescription(reward.OutputAugmentationKey);
        await message.ShowMessageAsync(description);
    }
}