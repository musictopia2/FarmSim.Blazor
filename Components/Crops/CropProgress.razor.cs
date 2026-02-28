namespace FarmSim.Blazor.Components.Crops;
public partial class CropProgress
{
    [Parameter]
    [EditorRequired]
    public Guid Id { get; set; }
    private string ReadyText => $"Ready In {CropManager.GetTimeLeft(Id)}";
    private string GrowingText => $"Growing 2 {CropManager.GetCropName(Id).GetWords}";
}