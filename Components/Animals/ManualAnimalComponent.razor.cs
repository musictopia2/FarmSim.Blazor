namespace FarmSim.Blazor.Components.Animals;
public partial class ManualAnimalComponent(IToast toast)
{
    [Parameter]
    [EditorRequired]
    public AnimalView Animal { get; set; } = default!;
    [Parameter] public bool UpgradeMode { get; set; }
    [Parameter] public EventCallback OnUpgraded { get; set; }
    private int _selectedIndex;
    private bool _showOptions;
    private bool _showUpgrades;
    private AnimalProductionOption? _selectedOption;
    protected override void OnParametersSet()
    {
        EnsureSelectedOption();
        base.OnParametersSet();
    }
    private void Upgraded()
    {
        _showUpgrades = false;
        OnUpgraded.InvokeAsync();
    }
    private void EnsureSelectedOption()
    {
        var options = AnimalManager.GetUnlockedProductionOptions(Animal);
        if (options.Count == 0)
        {
            _selectedOption = null;
            return;
        }

        if (_selectedIndex < 0 || _selectedIndex >= options.Count)
        {
            _selectedIndex = 0;
        }
        if (AdvancedSettingsManager.Settings.AnimalDefaultMode == EnumAnimalDefaultOption.Slowest)
        {
            _selectedIndex = options.Count - 1;
        }

        _selectedOption = options[_selectedIndex];
    }

    private EnumAnimalState State => AnimalManager.GetState(Animal);

    private bool CanOpenOptions => State == EnumAnimalState.None;
    private bool CanCollect => State == EnumAnimalState.Collecting && AnimalManager.Left(Animal) > 0;

    private bool _showProgress;

    private bool CanProduce
    {
        get
        {
            if (State != EnumAnimalState.None)
            {
                return false;
            }
            return AnimalManager.CanProduce(Animal, _selectedIndex);
        }
    }

    private void CardClicked()
    {
        if (UpgradeMode)
        {
            //do upgrade stuff instead.
            _showUpgrades = true;
            return;
        }
        if (CanCollect)
        {
            //may need a toast for animals.
            if (AnimalManager.CanCollect(Animal) == false)
            {
                toast.ShowUserErrorToast("Cannot collect from animal because barn is full.  Try crafting something, fulfilling orders, or discarding");
                return;
            }
            AnimalManager.Collect(Animal);
            return;
        }
        
        if (State == EnumAnimalState.Producing)
        {
            _showProgress = true;
            return;
        }
        if (CanProduce == false)
        {
            toast.ShowUserErrorToast("Unable to produce because you do not have enough required ingredients");
            return;
        }
        if (CanOpenOptions)
        {
            if (Animal.IsFast && Animal.MaxBenefits)
            {
                AnimalManager.Produce(Animal, 0);
                return;
            }
            if (AdvancedSettingsManager.Settings.AnimalDefaultMode == EnumAnimalDefaultOption.Fastest)
            {
                AnimalManager.Produce(Animal, 0);
                return;
            }
            if (AdvancedSettingsManager.Settings.AnimalDefaultMode == EnumAnimalDefaultOption.Slowest)
            {
                AnimalManager.Produce(Animal, _selectedIndex); //i think
                return;
            }
            _showOptions = true;
            return;
        }
    }
    private void ClosePopup() => _showOptions = false;

    // NEW: when an option is chosen, immediately start producing
    private void SelectOptionAndStart(int index)
    {
        int original = _selectedIndex;
        _selectedIndex = index;
        EnsureSelectedOption();
        ClosePopup();

        if (State != EnumAnimalState.None)
        {
            return;
        }

        if (AnimalManager.CanProduce(Animal, _selectedIndex) == false)
        {
            toast.ShowUserErrorToast("You don't have enough required ingredients for that option.");
            _selectedIndex = original;
            EnsureSelectedOption();
            return;
        }

        AnimalManager.Produce(Animal, _selectedIndex);
    }
    private string GetBackGround
    {
        get
        {
            var state = State;
            if (state == EnumAnimalState.Producing)
            {
                return cc1.DarkGray.ToWebColor;
            }
            if (state == EnumAnimalState.Collecting)
            {
                return cc1.Lime.ToWebColor;
            }
            if (CanProduce == false)
            {
                return cc1.DarkGray.ToWebColor;
            }
            return cc1.White.ToWebColor;
        }
    }


}