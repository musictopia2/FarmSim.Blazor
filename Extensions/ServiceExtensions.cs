namespace FarmSim.Blazor.Extensions;
public static class ServiceExtensions
{
    extension<P>(IServiceCollection services)
        where P : class, IPlayerNamePresenter
    {
        public IServiceCollection RegisterFarmSimBlazorServices()
        {
            services.AddSingleton<IPlayerNamePresenter, P>()
                .AddScoped<ReadyStatusService>()
                .AddScoped<OverlayService>()
                .AddScoped<FarmContext>()
                .AddScoped<INavigation, CustomNavigation>()
                ;
            return services;
        }
    }
}
