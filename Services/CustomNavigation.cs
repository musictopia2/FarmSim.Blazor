namespace FarmSim.Blazor.Services;
public class CustomNavigation(NavigationManager nav) : INavigation
{
    void INavigation.NavigateTo(FarmKey farm)
    {
        nav.NavigateTo(farm);
    }
}