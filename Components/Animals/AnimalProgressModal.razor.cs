namespace FarmSim.Blazor.Components.Animals;
public partial class AnimalProgressModal
{
    [Parameter]
    [EditorRequired]
    public AnimalView Animal { get; set; }
    [Parameter]
    [EditorRequired]
    public AnimalProductionOption Option { get; set; }
    private string ReadyText => $"Ready In {AnimalManager.TimeLeftForResult(Animal)}";
    private string ProducingText => $"Producing {AnimalManager.InProgress(Animal)} {Option.Output.Item.GetWords}";
    private string _rentalTimeLeft = "";
    protected override void OnInitialized()
    {
        Refresh();
        base.OnInitialized();
    }
    private void Refresh()
    {
        if (Animal.IsRental)
        {
            _rentalTimeLeft = RentalManager.GetDurationString(Animal.Name);
        }
    }

    protected override Task OnTickAsync()
    {
        Refresh();
        return base.OnTickAsync();
    }

}