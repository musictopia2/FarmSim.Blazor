using System.Reflection;

namespace FarmSim.Blazor.Startup;
public partial class CustomRouter
{
    private BasicList<string> _files = ["_content/FarmSim.Blazor/main.css"];
    public static string AppStyleName { get; set; } = "";
    //i like the possibility of allowing additional assmeblies even though you may not.
    private BasicList<Assembly> _additionalAssemblies = [];
    public static Assembly? AdditionalAssembly { get; set; }
    protected override void OnInitialized()
    {
        if (AdditionalAssembly is not null)
        {
            _additionalAssemblies =
                [
                    AdditionalAssembly
                ];
        }
        BlazoredToasts.Timeout = 3;
        ImageButtonComponent.FromResource = true;

    }

}