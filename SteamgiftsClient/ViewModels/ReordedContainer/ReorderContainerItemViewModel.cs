using Avalonia.Controls;
using ReactiveUI.Fody.Helpers;

namespace SteamgiftsClient.ViewModels.ReordedContainer
{
    public class ReorderContainerItemViewModel : ViewModelBase
    {
        [Reactive]
        public ContentControl ItemData { get; set; }

        public ReorderContainerItemViewModel(ContentControl itemData)
        {
            ItemData = itemData;
        }
    }
}
