namespace FarmSim.Blazor.Components.BaseClasses;
public abstract class FarmComponentBase : ComponentBase
{
    [CascadingParameter]
    protected MainFarmContainer? Farm { get; set; } //not common enough for the instant unlimited.  just do farm then what is needed this time.
    protected CropManager CropManager => Farm!.CropManager;
    protected TreeManager TreeManager => Farm!.TreeManager;
    protected WorkshopManager WorkshopManager => Farm!.WorkshopManager;
    protected AnimalManager AnimalManager => Farm!.AnimalManager;
    public WorksiteManager WorksiteManager => Farm!.WorksiteManager;
    protected InventoryManager InventoryManager => Farm!.InventoryManager;
    protected UpgradeManager UpgradeManager => Farm!.UpgradeManager;
    protected ProgressionManager ProgressionManager => Farm!.ProgressionManager;
    protected StoreManager StoreManager => Farm!.StoreManager;
    protected TimedBoostManager TimedBoostManager => Farm!.TimedBoostManager;
    protected ItemManager ItemManager => Farm!.ItemManager;
    protected RentalManager RentalManager => Farm!.RentalManager;
    protected ScenarioManager ScenarioManager => Farm!.ScenarioManager;
    protected AchievementManager AchievementManager => Farm!.AchievementManager;
    protected AdvancedSettingsManager AdvancedSettingsManager => Farm!.AdvancedSettingsManager;
    protected RulesManager RulesManager => Farm!.RulesManager;
    protected TreeAutomationManager TreeAutomationManager => Farm!.TreeAutomationManager;
    protected CropAutomationManager CropAutomationManager => Farm!.CropAutomationManager;
    protected AnimalAutomationManager AnimalAutomationManager => Farm!.AnimalAutomationManager;
    protected WorkshopAutomationManager WorkshopAutomationManager => Farm!.WorkshopAutomationManager;
    protected WorksiteAutomationManager WorksiteAutomationManager => Farm!.WorksiteAutomationManager;
    protected AdvancedUpgradeAutomationManager AdvancedUpgradeAutomationManager => Farm!.AdvancedUpgradeAutomationManager;
    public FarmKey Key => Farm!.FarmKey;
}