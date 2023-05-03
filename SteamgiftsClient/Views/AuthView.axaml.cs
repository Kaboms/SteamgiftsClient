using Avalonia.Controls;
using Avalonia.ReactiveUI;
using SteamgiftsClient.ViewModels;

namespace SteamgiftsClient.Views
{
    public partial class AuthView : ReactiveUserControl<AuthViewModel>
    {
        public AuthView()
        {
            InitializeComponent();
        }
    }
}
