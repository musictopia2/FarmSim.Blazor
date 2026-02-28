namespace FarmSim.Blazor.Models;
public sealed class ReadyStatusService
{
    public ReadyStatusModel Current { get; private set; } = new ReadyStatusModel();

    public event Action? OnChanged;

    public void Set(ReadyStatusModel next)
    {
        // prevent re-render storms
        if (Same(Current, next))
        {
            return;
        }

        Current = next;
        OnChanged?.Invoke();
    }

    private static bool Same(ReadyStatusModel a, ReadyStatusModel b) =>
        a.Crops == b.Crops &&
        a.Trees == b.Trees &&
        a.Animals == b.Animals &&
        a.Workshops == b.Workshops &&
        a.Worksites == b.Worksites;
}
