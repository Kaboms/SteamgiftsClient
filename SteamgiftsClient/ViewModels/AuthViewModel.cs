using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.Views.Windows;
using System.Reactive;
using System.Threading.Tasks;

namespace SteamgiftsClient.ViewModels
{
    public class AuthViewModel : ViewModelBase, IRoutableViewModel
    {
        #region Reactive commands
        public ReactiveCommand<Unit, Unit> ShowTutorialCommand { get; }
        public ReactiveCommand<Unit, bool> LoginCommand { get; }
        #endregion

        #region Reactive properties
        [Reactive]
        public string PHPSESID { get; set; } = string.Empty;

        [Reactive]
        public string LoginStatus { get; set; } = string.Empty;

        private readonly ObservableAsPropertyHelper<bool> _loginInProccess;
        public bool LoginInProccess
        {
            get { return _loginInProccess.Value; }
        }
        #endregion

        public string? UrlPathSegment => "auth";

        public IScreen HostScreen { get; }

        private ISiteManager _siteManager;

        public AuthViewModel(IScreen screen, ISiteManager siteManager)
        {
            HostScreen = screen;

            ShowTutorialCommand = ReactiveCommand.Create(ShowHelp);
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
            LoginCommand.IsExecuting.ToProperty(this, x => x.LoginInProccess, out _loginInProccess);

            _siteManager = siteManager;
        }

        private void ShowHelp()
        {
            TutorialWindow tutorialWindow = new TutorialWindow();
            tutorialWindow.Show();
        }

        private async Task<bool> Login()
        {
            LoginStatus = string.Empty;

            if (await _siteManager.LoginAsync(PHPSESID))
            {
                await Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>()
                    .MakeChangesAsync(preferences => preferences.PHPSESID = PHPSESID);

                return true;
            }

            LoginStatus = "Login failed";

            return false;
        }
    }
}
