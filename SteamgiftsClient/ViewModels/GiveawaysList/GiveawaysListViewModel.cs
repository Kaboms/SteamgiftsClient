using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace SteamgiftsClient.ViewModels
{
    public class GiveawaysListViewModel : ViewModelBase
    {
        #region Reactive Properties
        [Reactive]
        public bool IsLoading { get; set; } = true;

        private readonly ObservableAsPropertyHelper<bool> _inProccess;
        public bool InProccess
        {
            get { return _inProccess.Value; }
        }

        private readonly ReadOnlyObservableCollection<GiveawayViewModel>? _giveawaysViewModels;
        public ReadOnlyObservableCollection<GiveawayViewModel>? GiveawaysViewModels => _giveawaysViewModels;

        private SourceCache<GiveawayViewModel, String> _sourceCache = new(x => x.GiveawayData.ID);

        [Reactive]
        public PageNavigatorViewModel PageNavigatorViewModel { get; set; } = new PageNavigatorViewModel(5, 1);
        #endregion

        public ReactiveCommand<Unit, Unit> EnterToAll { get; }

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

            EnterToAll = ReactiveCommand.CreateFromTask(AsyncEnterToAll);
            EnterToAll.IsExecuting.ToProperty(this, x => x.InProccess, out _inProccess);

        }

        public async Task AsyncEnterToAll()
        {
            UserPreferences userPreferences = Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>().GetPreferences();

            foreach (SearchCategory category in userPreferences.EntryCategoriesOrder)
            {
                int page = 1;
                while (true)
                {
                    if (_siteManager.UserInfo.Points < 10)
                        return;

                    List<Giveaway> giveaways = await _siteManager.GetGiveawaysAsync(SearchCategory.All, page);
                    if (giveaways.Count == 0)
                    {
                        break;
                    }

                    foreach (Giveaway giveaway in giveaways)
                    {
                        if (giveaway.Cost <= _siteManager.UserInfo.Points)
                        {
                            await _siteManager.EnterGiveawayAsync(giveaway);

                            await Task.Delay(777);
                        }
                    }

                    ++page;
                }
            }
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
