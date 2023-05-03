using Avalonia.FreeDesktop.DBusIme;
using Avalonia.Threading;
using ReactiveUI;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using System;

namespace SteamgiftsClient.ViewModels.Windows
{
    public class MainWindowViewModel : ViewModelBase, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        private readonly ISiteManager _siteManager;

        public MainWindowViewModel(IPreferencesManager<UserPreferences> userPreferencesManager, ISiteManager siteManager)
        {
            _siteManager = siteManager;

            if (string.IsNullOrEmpty(userPreferencesManager.GetPreferences().PHPSESID))
            {
                ShowAuthView();
            }
            else
            {
                _siteManager.LoginAsync(userPreferencesManager.GetPreferences().PHPSESID)
                    .ContinueWith(x =>
                    {
                        if (x.Result == true)
                            ShowContentView();
                        else
                            ShowAuthView();
                    });
            }
        }

        public void ShowAuthView()
        {
            var authViewModel = new AuthViewModel(this, _siteManager);
            authViewModel.LoginCommand.Subscribe(result =>
            {
                if (result == true)
                    ShowContentView();
            });
            Router.Navigate.Execute(new AuthViewModel(this, _siteManager));
        }

        public void ShowContentView()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var contentViewModel = new ContentViewModel(this, _siteManager);
                Router.Navigate.Execute(contentViewModel);
            });

        }
    }
}
