using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using SteamgiftsClient.Views.ReordedContainer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SteamgiftsClient.ViewModels.ReordedContainer
{
    public class ReordedContainerViewModel : ViewModelBase
    {
        public ObservableCollection<ReorderContainerItemView> Items { get; } = new ObservableCollection<ReorderContainerItemView>();

        public ReordedContainerViewModel()
        {
            var vm1 = new ReorderContainerItemViewModel(new Label { Content = "1" });
            var v1 = new ReorderContainerItemView { DataContext = vm1 };

            var vm2 = new ReorderContainerItemViewModel(new Label { Content = "2" });
            var v2 = new ReorderContainerItemView { DataContext = vm2 };

            var vm3 = new ReorderContainerItemViewModel(new Label { Content = "3" });
            var v3 = new ReorderContainerItemView { DataContext = vm3 };

            Items.AddRange(new List<ReorderContainerItemView> { v1, v2, v3});
        }
    }
}
