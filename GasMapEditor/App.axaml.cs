using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GasMapEditor.Helpers;
using GasMapEditor.Services;
using GasMapEditor.ViewModels;
using GasMapEditor.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GasMapEditor
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IServiceProvider Services { get; private set; }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = new MainWindow();

                var services = new ServiceCollection();
                services.AddServices();
                services.AddSingleton<IFileService>(_ => new FileService(desktop.MainWindow));
                services.AddSingleton<IPopupService>(_ => new PopupWindowService(desktop.MainWindow));

                Services = services.BuildServiceProvider();

                var mainViewModel = Services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow.DataContext = DataContext = mainViewModel;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}