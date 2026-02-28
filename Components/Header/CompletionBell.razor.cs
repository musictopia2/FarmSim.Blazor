namespace FarmSim.Blazor.Components.Header;
public partial class CompletionBell(ReadyStatusService readyService)
{
    private bool _treesReady;
    private bool _animalsReady;
    private bool _cropsReady;
    private bool _workshopsReady;
    private bool _worksiteReady;
    protected override void OnInitialized()
    {
        PossibleUpdate();
        base.OnInitialized();
    }

    private void PossibleUpdate()
    {
        _treesReady = false;
        _animalsReady = false;
        _cropsReady = false;
        _workshopsReady = false;
        _worksiteReady = false;
        var workshops = WorkshopManager.GetUnlockedWorkshops;
        if (workshops.Any(x => x.ReadyCount > 0))
        {
            _workshopsReady = true;
        }
        var worksites = WorksiteManager.GetUnlockedWorksites();
        foreach (var item in worksites )
        {
            if (WorksiteManager.GetStatus(item) == EnumWorksiteState.Collecting)
            {
                _worksiteReady = true;
                break;
            }
        }
        var animals = AnimalManager.GetUnlockedAnimals;
        foreach (var item in animals )
        {
            if (AnimalManager.GetState(item) == EnumAnimalState.Collecting)
            {
                _animalsReady = true;
                break;
            }
        }
        var trees = TreeManager.GetUnlockedTrees;
        foreach (var item in trees)
        {
            var ready = TreeManager.TreesReady(item);
            TimeSpan time = TreeManager.TreeDuration(item);
            if (time.TotalSeconds > 60)
            {
                if (ready > 0)
                {
                    _treesReady = true;
                    break;
                }
            }    
            
        }
        var crops = CropManager.GetUnlockedCrops;
        foreach (var item in crops)
        {
            if (CropManager.GetCropState(item) == EnumCropState.Ready)
            {
                _cropsReady = true;
                break;
            }
        }
        readyService.Set(new ReadyStatusModel
        {
            Crops = _cropsReady,
            Trees = _treesReady,
            Animals = _animalsReady,
            Workshops = _workshopsReady,
            Worksites = _worksiteReady
        });

    }


    protected override Task OnTickAsync()
    {
        PossibleUpdate();
        return base.OnTickAsync();
    }
}