namespace FarmSim.Blazor.Services;
public sealed class DefaultPlayerNamePresenter : IPlayerNamePresenter
{
    private static readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Player1"] = "Player1",
        ["Player 1"] = "Player1",
        ["player1"] = "Player1",

        ["Player2"] = "Player2",
        ["Player 2"] = "Player2",
        ["player2"] = "Player2",
    };

    public string ToDisplayName(string playerKey)
        => _map.TryGetValue(playerKey ?? "", out var name) ? name : (playerKey ?? "");
}