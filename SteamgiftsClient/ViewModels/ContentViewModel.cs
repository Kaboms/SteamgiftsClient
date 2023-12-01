using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.ViewModels.ReordedContainer;
using System.Linq;
using System.Reactive;

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

        public ReactiveCommand<Unit, Unit> ApplyFilterCommand { get; }

        public string? UrlPathSegment => "main";

        public IScreen HostScreen { get; }

        public ContentViewModel(IScreen screen, ISiteManager siteManager)
        {
            HostScreen = screen;

            UserInfoViewModel = new UserInfoViewModel(siteManager);
            GiveawaysListViewModel = new GiveawaysListViewModel(siteManager);

            ApplyFilterCommand = ReactiveCommand.Create(AsyncApplyFilter);
        }

        private async void AsyncApplyFilter()
        {
            await Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>()
                .MakeChangesAsync(x => x.EntryCategoriesOrder = ReorderContainerViewModel.OrderedItems.Select(x => x.ViewModel.Category).ToList());
        }
    }
}
