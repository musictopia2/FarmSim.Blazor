namespace FarmSim.Blazor.Components.Header;
public partial class LevelDetailsModal(IToast toast)
{
    private string PointsToLevelUp => ProgressionManager.PointsNeededToLevelUp.ToString("N0");
    private static string GetRewardImageName(string name)
    {
        if (name.Contains("trees", StringComparison.CurrentCultureIgnoreCase))
        {
            return Images.Standard.Assets.PngImageAssets.Tree;
        }
        return name;
    }

    //private static string ImagePath(string name) => $"/{name}.png";
    private void ShowText(string key) => toast.ShowInfoToast(key.GetWords);

    //private static string GetRewardImageName(string key)
    //{
    //    // Treat anything ending in Trees as a tree unlock reward
    //    // PeachTrees, AppleTrees, etc.
    //    if (key.EndsWith("Trees", StringComparison.OrdinalIgnoreCase))
    //        return "Tree"; // <- make sure you DO have a generic Tree image

    //    return key;
    //}

    private string GetDetails
    {
        get
        {
            string levelText = PointsToLevelUp;
            int nextLevel = ProgressionManager.NextLevel;
            return $"{levelText} more points to each level {nextLevel} to unlock:";


        }
    }
    

}