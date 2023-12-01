using Avalonia.Controls;
using ReactiveUI.Fody.Helpers;
using SteamgiftsClient.Services.SiteManager;

namespace SteamgiftsClient.ViewModels.ReordedContainer
{
    public class ReorderContainerItemViewModel : ViewModelBase
    {
        [Reactive]
        public ContentControl ItemData { get; set; }

        public ReordedContainerViewModel ParentContainer;

        public SearchCategory Category { get; set; }

        public ReorderContainerItemViewModel(ContentControl itemData, ReordedContainerViewModel parentContainer, SearchCategory category)
        {
            ItemData = itemData;
            ParentContainer = parentContainer;
            Category = category;
        }
    }
}
