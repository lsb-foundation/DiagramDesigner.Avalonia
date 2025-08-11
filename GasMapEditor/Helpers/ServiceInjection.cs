using GasMapEditor.Services;
using GasMapEditor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GasMapEditor.Helpers;

internal static class ServiceInjection
{
    public static IServiceCollection AddServices(this IServiceCollection @this) =>
        @this.AddSingleton<GasMapDriver>()
            .AddSingleton<PreviewWindowViewModel>()
            .AddSingleton<MainWindowViewModel>();
}
