using Avalonia.Controls.Shapes;
using ReactiveUI;
using Splat;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.ViewModels;
using SteamgiftsClient.ViewModels.Windows;
using SteamgiftsClient.Views;
using System;
using System.IO;

namespace SteamgiftsClient.Common
{
    public static class AppBootstraper
    {
        /// <summary>
        /// Get service from Locator. Throw InvalidOperationException if failed resovle service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="resolver"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
        {
            var service = resolver.GetService<TService>();
            if (service is null)
            {
                // Splat is not able to resolve type for us
                throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
            }

            return service;
        }

        public static void Bootstrap()
        {
            if (!Directory.Exists(App.DataFolder))
                Directory.CreateDirectory(App.DataFolder);

            InitDI();
        }

        private static void InitDI()
        {
            Locator.CurrentMutable.RegisterConstant<ISiteManager>(new SiteManager());
            Locator.CurrentMutable.RegisterConstant<IPreferencesManager<UserPreferences>>(
                    new PreferencesJsonManager<UserPreferences>("preferences.json")
                );

            RegisterViewModelsDI();
        }

        /// <summary>
        /// Register DI for IScreen and IViewFor
        /// </summary>
        private static void RegisterViewModelsDI()
        {
            Locator.CurrentMutable.Register<IScreen>(() => new MainWindowViewModel(
                            Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>(),
                            Locator.Current.GetRequiredService<ISiteManager>()
                ));

            Locator.CurrentMutable.Register<IViewFor<AuthViewModel>>(() => new AuthView());
            Locator.CurrentMutable.Register<IViewFor<ContentViewModel>>(() => new ContentView());
        }
    }
}
