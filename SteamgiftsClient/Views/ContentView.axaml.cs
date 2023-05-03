using Avalonia.Controls;
using Avalonia.ReactiveUI;
using SteamgiftsClient.ViewModels;

namespace SteamgiftsClient.Views
{
    public partial class ContentView : ReactiveUserControl<ContentViewModel>
    {
        public ContentView()
        {
            InitializeComponent();
        }
    }
}
