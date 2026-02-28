namespace FarmSim.Blazor.Components.Header;
public partial class CoinIndicator(NavigationManager nav)
{
    private int CoinCount => InventoryManager.Get(CurrencyKeys.Coin);
    private string CoinCountDisplay => CoinCount.ToString("N0");
    private void AttemptEarnCoin()
    {
        if (this.IsCooperative)
        {
            return;  //only main can do this.
        }
        if (RulesManager.AutomationEnabled)
        {
            return; //looks like automation mode does not work anymore for the scenarios.
        }
        FarmKey farm = this.AsCoin;
        nav.NavigateTo(farm);
    }
}