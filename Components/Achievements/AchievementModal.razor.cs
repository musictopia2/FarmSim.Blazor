namespace FarmSim.Blazor.Components.Achievements;
public partial class AchievementModal
{
    private BasicList<AchievementUiRow> _current = [];
    private BasicList<AchievementUiRow> _completed = [];
    protected override void OnInitialized()
    {
        _current = AchievementManager.GetCurrentAchievementsForUi();
        _completed = AchievementManager.GetCompletedAchievementsForUi();
        base.OnInitialized();
    }
}