namespace FarmSim.Blazor.Components.Achievements;
public partial class CompletionStatsComponent
{
    private BasicList<AchievementUiRow> _current = [];
    protected override void OnInitialized()
    {
        _current = AchievementManager.GetCurrentAchievementsForUi();
        _current.RemoveAllOnly(x => x.IsRepeatable == false);
        _current.KeepConditionalItems(x => x.Current > 0);
        base.OnInitialized();
    }
}