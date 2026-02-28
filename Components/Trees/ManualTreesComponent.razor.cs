namespace FarmSim.Blazor.Components.Trees;

public partial class ManualTreesComponent
{
    private BasicList<TreeView> _trees = [];
    protected override void OnInitialized()
    {
        Refresh();
        base.OnInitialized();
    }
    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }
    private void Refresh()
    {
        // IMPORTANT: in automation mode TreeManager doesn't exist.
        _trees = TreeManager.GetUnlockedTrees;

    }
}