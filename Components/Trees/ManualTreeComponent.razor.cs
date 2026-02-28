namespace FarmSim.Blazor.Components.Trees;
public partial class ManualTreeComponent(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public TreeView Tree { get; set; }
    private int _ready;
    private bool _showpopup = false;
    private bool _showUpgrades = false;
    private string _rentalTimeLeft = "";
    protected override void OnInitialized()
    {
        Refresh();

        base.OnInitialized();
    }
    private void Refresh()
    {
        _ready = TreeManager.TreesReady(Tree);
        if (Tree.IsRental)
        {
            _rentalTimeLeft = RentalManager.GetDurationString(Tree.TreeName);
        }
    }

    private void OpenUpgrades()
    {
        _showpopup = false;
        _showUpgrades = true;
    }

    private void Upgraded()
    {
        _showUpgrades = false;
    }

    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }
    private void Process()
    {
        if (_ready > 0)
        {
            if (TreeManager.CanCollectFromTree(Tree) == false)
            {
                toast.ShowUserErrorToast("Unable to collect from trees because not enough room in your silo.  Try discarding some items, craft something, or fulfill orders");
                return;
            }
            TreeManager.CollectFromTree(Tree);
            _ready = TreeManager.TreesReady(Tree);
            return;
        }
        _showpopup = true;
    }
}