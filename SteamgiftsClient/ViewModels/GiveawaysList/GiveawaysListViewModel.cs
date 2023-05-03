using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using SteamgiftsClient.Services.SiteManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamgiftsClient.ViewModels
{
    public class GiveawaysListViewModel : ViewModelBase
    {
        #region Reactive Properties
        [Reactive]
        public bool IsLoading { get; set; } = true;

        private readonly ReadOnlyObservableCollection<GiveawayViewModel>? _giveawaysViewModels;
        public ReadOnlyObservableCollection<GiveawayViewModel>? GiveawaysViewModels => _giveawaysViewModels;

        private SourceCache<GiveawayViewModel, String> _sourceCache = new(x => x.GiveawayData.ID);

        [Reactive]
        public PageNavigatorViewModel PageNavigatorViewModel { get; set; } = new PageNavigatorViewModel(5, 1);
        #endregion

        private readonly ISiteManager _siteManager;

        public GiveawaysListViewModel(ISiteManager siteManager)
        {
            _siteManager = siteManager;

            _sourceCache.Connect()
                .Bind(out _giveawaysViewModels)
                .Subscribe();

            LoadGiveawaysAsync();

            PageNavigatorViewModel.WhenPropertyChanged(x => x.SelectedPage, notifyOnInitialValue: false)
                .Subscribe((selectedPage) =>
                {
                    LoadGiveawaysAsync(selectedPage.Value);
                });
        }

        public async void LoadGiveawaysAsync(int page = 1)
        {
            IsLoading = true;

            if (PageNavigatorViewModel.PagesCount <= page && await _siteManager.HasNextPage(page))
            {
                PageNavigatorViewModel.PagesCount = page + 1;
            }

            _sourceCache.Clear();
            _sourceCache.AddOrUpdate((await _siteManager.GetGiveawaysAsync(SearchCategory.All, page)).Select(x =>
            {
                return new GiveawayViewModel(x);
            }));

            IsLoading = false;
        }
    }
}
