using FarmSim.Blazor.Components.BaseClasses;

namespace FarmSim.Blazor.Extensions;
public static class FarmExtensions
{
    
    extension(FarmComponentBase farm)
    {
        public FarmKey AsMain => farm.Key with { Slot = EnumFarmSlot.Main };
        public FarmKey AsCoin => farm.Key with { Slot = EnumFarmSlot.Coin };
        public FarmKey AsCooperative => farm.Key with { Slot = EnumFarmSlot.Cooperative };
        public bool IsMain => farm.Key.Slot == EnumFarmSlot.Main;
        public bool IsCoin => farm.Key.Slot == EnumFarmSlot.Coin;
        public bool IsCooperative => farm.Key.Slot == EnumFarmSlot.Cooperative;
        public bool IsBaseline => farm.Key.IsMain || farm.Key.IsCooperative;
    }
    extension(NavigationManager nav)
    {
        public void NavigateTo(FarmKey key)
        {
            nav.NavigateTo($"/farm/{key.Theme}/{key.PlayerName}/{key.ProfileId}/{key.Slot}", forceLoad: true);
        }
    }

    extension(FarmComponentBase farm)
    {
        public bool CanCloseWorksiteAutomatically(string? location)
        {
            return farm.WorksiteManager.CanCloseWorksiteAutomatically(location);

        }
    }
    extension(FarmContext farm)
    {
        public bool CanCloseWorksiteAutomatically(string? location)
        {
            return farm.Current!.WorksiteManager.CanCloseWorksiteAutomatically(location);
        }
    }
    extension(WorksiteManager manager)
    {
        public bool CanCloseWorksiteAutomatically(string? location)
        {
            if (manager is null)
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(location))
            {
                return true; //try this way (?)
            }
            EnumWorksiteState status = manager.GetStatus(location);
            if (status != EnumWorksiteState.Collecting)
            {
                return true;
            }
            if (manager.CanCollectRewards(location))
            {
                if (manager.CanCollectRewardsWithLimits(location) == false)
                {
                    return true; //because you have limits
                }
                return false;
            }
            return true;
        }
    }


}