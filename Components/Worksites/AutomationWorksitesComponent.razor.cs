namespace FarmSim.Blazor.Components.Worksites;
public partial class AutomationWorksitesComponent(OverlayService overlay)
{
    private BasicList<string> _worksites = [];

    protected override void OnInitialized()
    {
        Refresh();
        base.OnInitialized();
    }

    private void Refresh()
    {
        _worksites = WorksiteAutomationManager.GetUnlockedWorksites();
    }

    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }

    private async Task SelectWorksiteAsync(string site)
    {
        await overlay.OpenPossibleWorksiteAsync(site);
    }

}
