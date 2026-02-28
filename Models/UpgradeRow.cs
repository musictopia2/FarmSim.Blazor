namespace FarmSim.Blazor.Models;
public sealed record UpgradeColumn(
    int LevelDesired, //starts with 2.
    int LevelRequired,
    Dictionary<string, int> Costs

);