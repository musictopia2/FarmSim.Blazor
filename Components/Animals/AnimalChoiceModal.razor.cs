namespace FarmSim.Blazor.Components.Animals;
public partial class AnimalChoiceModal(OverlayService overlay)
{
    [Parameter]
    [EditorRequired]
    public AnimalView Animal { get; set; } = default!;

    [Parameter]
    public EventCallback<int> OnOptionChosen { get; set; }

    private BasicList<AnimalProductionOption> _productionOptions = [];

    private AnimalPreviewOption? _nextOption;
    private string _rentalTimeLeft = "";

    private async Task ClickedCropAsync(string name)
    {
        if (name == "")
        {
            return;
        }
        //just clicking means needs to open the crops.
        await overlay.OpenCropsAsync();

    }

    protected override void OnParametersSet()
    {
        _productionOptions = AnimalManager.GetUnlockedProductionOptions(Animal);
        _nextOption = ProgressionManager.NextAnimalOption(Animal.Name);
        if (Animal.IsRental)
        {
            _rentalTimeLeft = RentalManager.GetDurationString(Animal.Name);
        }


        //figure out next option.


    }

    private void OptionSelected(AnimalProductionOption option)
    {
        int index = _productionOptions.IndexOf(option);
        OnOptionChosen.InvokeAsync(index);
    }

    // --- Image path helper (edit to match your project) ---
    private static string Normalize(string text)
        => text.Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();

    private static string GetItemImage(string itemName)
        => $"/{Normalize(itemName)}.png";


    private string GetDurationText(AnimalProductionOption option)
    {
        
        int index = _productionOptions.IndexOf(option);
        return AnimalManager.Duration(Animal, index);
    }

    private int Have(string itemKey) => InventoryManager.Get(itemKey);



}