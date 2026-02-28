namespace FarmSim.Blazor.Models;
public class ReadyStatusModel
{
    public bool Crops { get; init; }
    public bool Trees { get; init; }
    public bool Animals { get; init; }
    public bool Workshops { get; init; }
    public bool Worksites { get; init; }
}