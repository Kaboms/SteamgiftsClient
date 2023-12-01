using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.ViewModels.Windows;
using SteamgiftsClient.Views;
using System;
using System.Collections.Generic;
using System.IO;

namespace SteamgiftsClient
{
    public partial class App : Application
    {
        public static string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SteamgiftsClient");

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            AppBootstraper.Bootstrap();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>()
                    .SetDefaultPreferences(new UserPreferences { EntryCategoriesOrder = new List<SearchCategory> { SearchCategory.Wishlist, SearchCategory.Recommended, SearchCategory.All } });

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(
                            Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>(),
                            Locator.Current.GetRequiredService<ISiteManager>()
                        ),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
