namespace FarmSim.Blazor.Components.Worksites;
public partial class ShowWorksiteModal
{
    [Parameter]
    public bool Visible { get; set; }
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter]
    public string? Location { get; set; }
    [Parameter]
    public EventCallback<string?> LocationChanged { get; set; }
    private BasicList<string> _worksites = [];

    protected override void OnParametersSet()
    {
        if (WorksiteManager is not null)
        {
            _worksites = WorksiteManager.GetUnlockedWorksites();
        }
        else
        {
            _worksites = WorksiteAutomationManager.GetUnlockedWorksites();
        }
        base.OnParametersSet();
    }
    private void Next()
    {
        if (Location is null)
        {
            return;
        }
        int index = _worksites.IndexOf(Location);
        if (index == _worksites.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        Location = _worksites[index];
        LocationChanged.InvokeAsync(Location);
    }
    private void Previous()
    {
        if (Location is null)
        {
            return;
        }
        int index = _worksites.IndexOf(Location);
        if (index == 0)
        {
            index = _worksites.Count - 1;
        }
        else
        {
            index--;
        }
        Location = _worksites[index];
        LocationChanged.InvokeAsync(Location);
    }

    private void CloseWorksite()
    {
        Visible = false;
        VisibleChanged.InvokeAsync();
        Location = null;
        LocationChanged.InvokeAsync(Location);
    }

    private bool CanCloseWorksiteAutomatically
    {
        get
        {
            return this.CanCloseWorksiteAutomatically(Location);

        }
    }


}