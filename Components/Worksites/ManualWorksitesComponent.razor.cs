namespace FarmSim.Blazor.Components.Worksites;
public partial class ManualWorksitesComponent(OverlayService overlay)
{
    private BasicList<string> _worksites = [];
    //this needs to know when there is something to collect.
    override protected void OnInitialized()
    {
        UpdateWorksites();
        base.OnInitialized();
        //WorksiteManager.OnWorksitesUpdated += Refresh;
    }

    private void UpdateWorksites()
    {
        _worksites = WorksiteManager.GetUnlockedWorksites();
    }
    private void Refresh()
    {
        UpdateWorksites();
    }
    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }
    //[Parameter]
    //public EventCallback<string> WorksiteSelected { get; set; }
    private async Task SelectWorksiteAsync(string site)
    {
        await overlay.OpenPossibleWorksiteAsync(site);
        //if (WorksiteSelected.HasDelegate)
        //{
        //    WorksiteSelected.InvokeAsync(site);
        //}
    }
    private static string Image(string workshop) => $"/{workshop}.png";
    //public void Dispose()
    //{
    //    WorksiteManager.OnWorksitesUpdated -= Refresh;
    //    GC.SuppressFinalize(this);
    //}
}