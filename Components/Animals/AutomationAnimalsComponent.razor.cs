namespace FarmSim.Blazor.Components.Animals;
public partial class AutomationAnimalsComponent
{
    private AnimalAutomationView? _animal;
    private BasicList<AnimalAutomationView> _animals = [];
    protected override void OnInitialized()
    {
        _animals = AnimalAutomationManager.GetUnlockedAnimals;
    }

    private void OnCLick(AnimalAutomationView animal)
    {
        _animal = animal;
    }
    private void Previous()
    {
        if (_animal is null)
        {
            return;
        }
        int index = _animals.IndexOf(_animal);
        if (index == 0)
        {
            index = _animals.Count - 1;
        }
        else
        {
            index--;
        }
        _animal = _animals[index];
    }
    private void Next()
    {
        if (_animal is null)
        {
            return;
        }
        int index = _animals.IndexOf(_animal);
        if (index == _animals.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        _animal = _animals[index];
    }
}