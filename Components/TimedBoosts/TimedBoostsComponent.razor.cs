namespace FarmSim.Blazor.Components.TimedBoosts;
public partial class TimedBoostsComponent(IToast toast, IMessageBox message)
{
    private BasicList<TimedBoostCredit> _credits = [];
    protected override void OnInitialized()
    {
        Refresh();
        base.OnInitialized();
    }
    private async Task GiveDetailsAsync(TimedBoostCredit credit)
    {
        if (credit.OutputAugmentationKey is null)
        {
            return;
        }
        string description = Farm!.OutputAugmentationManager.GetDescription(credit.OutputAugmentationKey);
        await message.ShowMessageAsync(description);
    }

    private void Refresh()
    {
        _credits = TimedBoostManager.GetBoosts();
    }


    private static string GetBoostTitle(string boostKey)
        => boostKey.GetWords;

    private bool CanActivateBoost(TimedBoostCredit credit) => TimedBoostManager.CanActivateBoost(credit);
    private async Task ActivateBoostAsync(TimedBoostCredit credit)
    {
        if (credit.Quantity <= 0)
        {
            return;
        }
        if (CanActivateBoost(credit) == false)
        {
            toast.ShowUserErrorToast("Unable to activate boost");
            return;
        }
        await TimedBoostManager.ActiveBoostAsync(credit);
        Refresh();
    }
}