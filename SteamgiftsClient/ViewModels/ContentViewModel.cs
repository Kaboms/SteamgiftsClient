using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.ViewModels.ReordedContainer;

namespace SteamgiftsClient.ViewModels
{
    public class ContentViewModel : ViewModelBase, IRoutableViewModel
    {
        #region Reactive Properties
        [Reactive]
        public UserInfoViewModel UserInfoViewModel { get; set; }

        [Reactive]
        public GiveawaysListViewModel GiveawaysListViewModel { get; set; }

        [Reactive]
        public ReordedContainerViewModel ReorderContainerViewModel { get; set; } = new ReordedContainerViewModel();
        #endregion

        public string? UrlPathSegment => "main";

        public IScreen HostScreen { get; }

        public ContentViewModel(IScreen screen, ISiteManager siteManager)
        {
            HostScreen = screen;

            UserInfoViewModel = new UserInfoViewModel(siteManager);
            GiveawaysListViewModel = new GiveawaysListViewModel(siteManager);
        }
    }
}
