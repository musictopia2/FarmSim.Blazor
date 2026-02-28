namespace FarmSim.Blazor.Components.Trees;
public partial class AutomationTreesComponent
{
    private BasicList<TreeAutomationView> _trees = [];

    private TreeAutomationView? _selectedTree;
    protected override void OnInitialized()
    {
        _trees = TreeAutomationManager.GetUnlockedTrees;
        base.OnInitialized();
    }

    private void SelectTree(TreeAutomationView tree)
    {
        _selectedTree = tree;
    }
    private void Previous()
    {
        if (_selectedTree is null)
        {
            return;
        }
        int index = _trees.IndexOf(_selectedTree);
        if (index == 0)
        {
            index = _trees.Count - 1;
        }
        else
        {
            index--;
        }
        _selectedTree = _trees[index];
    }
    private void Next()
    {
        if (_selectedTree is null)
        {
            return;
        }
        int index = _trees.IndexOf(_selectedTree);
        if (index == _trees.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        _selectedTree = _trees[index];
    }

}