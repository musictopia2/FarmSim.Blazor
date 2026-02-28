namespace FarmSim.Blazor.Components.Header;
public partial class LevelIndicator(OverlayService overlay) : IDisposable
{
    private string Level => ProgressionManager.CurrentLevel.ToString("N0");
    private int CurrentPoints => ProgressionManager.CurrentPoints;
    private int PointsRequired => ProgressionManager.PointsRequired;
    protected override void OnInitialized()
    {
        ProgressionManager.Changed += OnProgressionChanged;
        base.OnInitialized();
    }

    private async Task OpenLevelDetailsAsync()
    {
        await overlay.OpenLevelsAsync();
    }

    private void OnProgressionChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private double Progress01
    {
        get
        {
            if (PointsRequired <= 0) return 0;
            var v = (double)CurrentPoints / PointsRequired;
            if (v < 0) return 0;
            if (v > 1) return 1;
            return v;
        }
    }
    private BasicList<int> Milestones => ProgressionManager.GetCompleteThresholds();

    private string MilestoneLeft(int milestonePoints)
    {
        if (PointsRequired <= 0)
        {
            return "0%";
        }

        // clamp to [0..1] then convert to percent
        var p = (double)milestonePoints / PointsRequired;
        if (p < 0) p = 0;
        if (p > 1) p = 1;

        return $"{p * 100:0.##}%";
    }

    private string ProgressPercent => $"{Progress01 * 100:0.##}%";
    private string ProgressText => $"{CurrentPoints:N0}/{PointsRequired:N0}";

    public void Dispose()
    {
        ProgressionManager.Changed -= OnProgressionChanged;
        GC.SuppressFinalize(this);
    }
}