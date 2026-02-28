namespace FarmSim.Blazor.Components.TimedBoosts;
public partial class ActiveEffectsOverlay(IMessageBox message) : IDisposable
{
    private BasicList<ActiveTimedBoost> _effects = [];
    protected override void OnInitialized()
    {
        TimedBoostManager.Tick += Refresh;
        _effects = TimedBoostManager.GetActiveBoosts;
        base.OnInitialized();
    }
    private void Refresh()
    {
        _effects = TimedBoostManager.GetActiveBoosts;
        InvokeAsync(StateHasChanged);
    }
    private async Task GiveDetailsAsync(ActiveTimedBoost boost)
    {
        if (boost.OutputAugmentationKey is null)
        {
            return;
        }
        await message.ShowMessageAsync(Farm!.OutputAugmentationManager.GetDescription(boost.OutputAugmentationKey));
    }
    public void Dispose()
    {
        TimedBoostManager.Tick -= Refresh;
        GC.SuppressFinalize(this);
    }
    public static string GetTimeLeft(ActiveTimedBoost boost)
    {
        
        var remaining = boost.EndsAt - DateTime.Now;
        if (remaining <= TimeSpan.Zero)
        {
            return ""; // already expired
        }
        return remaining.GetTimeCompact;
    }

    private static string GetDetails(ActiveTimedBoost b)
    {
        // If ReduceBy is set, show that.
        if (b.ReduceBy.HasValue)
        {
            

            return $"Time reduction: –{b.ReduceBy.Value.GetTimeString}";
        }


        // Otherwise, describe by BoostKey (customize as you add boost types)
        return b.BoostKey switch
        {
            "UnlimitedSpeedSeeds" => "Unlimited instant harvest while active",
            _ => "Timed effect active"
        };
    }
    
    private static bool ShouldShowSubLine(ActiveTimedBoost b)
    {
        // Since ReduceBy is already shown visually in BoostIconComponent,
        // don’t repeat it in text.
        if (b.ReduceBy.HasValue)
        {
            return false;
        }

        // If UnlimitedSpeedSeeds icon already communicates it, you can hide text too.
        if (b.BoostKey == "UnlimitedSpeedSeeds")
        {
            return false;
        }

        return true;
    }

}