using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Common.Helpers;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.PreferencesManager;
using SteamgiftsClient.Services.SiteManager;
using SteamgiftsClient.Views.ReordedContainer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamgiftsClient.ViewModels.ReordedContainer
{
    public class ReordedContainerViewModel : ViewModelBase
    {
        public ObservableCollection<ReorderContainerItemView> Items { get; } = new ObservableCollection<ReorderContainerItemView>();

        public List<Rect> ItemsBounds = new List<Rect>();

        public List<ReorderContainerItemView> OrderedItems = new List<ReorderContainerItemView>();

        public ReordedContainerViewModel()
        {
            UserPreferences userPreferences = Locator.Current.GetRequiredService<IPreferencesManager<UserPreferences>>().GetPreferences();

            foreach (SearchCategory searchCategory in userPreferences.EntryCategoriesOrder)
            {
                AddItem(searchCategory);
            }

            OrderedItems = Items.ToList();
        }

        private void AddItem(SearchCategory category)
        {
            var newItemViewModel = new ReorderContainerItemViewModel(new Label { Content = category.ToString() }, this, category);
            var newItemView = new ReorderContainerItemView { DataContext = newItemViewModel };

            Items.Add(newItemView);
        }

        public void CheckIntersections(ReorderContainerItemView checkItem)
        {
            int checkItemIndex = OrderedItems.IndexOf(checkItem);
            for (int i = 0; i < OrderedItems.Count; i++)
            {
                if (i != checkItemIndex)
                {
                    if (ItemsBounds[i].Contains(checkItem.GetCurrentBounds().Center))
                    {
                        var newTransform = new TranslateTransform(0, ItemsBounds[checkItemIndex].Y - OrderedItems[i].Bounds.Y);
                        OrderedItems[i].SetTransform(newTransform);

                        OrderedItems.Swap(checkItemIndex, i);

                        break;
                    }
                }
            }
        }

        public void SetItemDefaultBounds(ReorderContainerItemView checkItem)
        {
            int checkItemIndex = OrderedItems.IndexOf(checkItem);
            OrderedItems[checkItemIndex].SetTransform(new TranslateTransform(0, ItemsBounds[checkItemIndex].Y - OrderedItems[checkItemIndex].Bounds.Y));
        }
    }
}
