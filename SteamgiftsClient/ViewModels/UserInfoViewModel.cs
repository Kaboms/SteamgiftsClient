using Avalonia.Media.Imaging;
using ReactiveUI.Fody.Helpers;
using SteamgiftsClient.Common.Helpers;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.SiteManager;

namespace SteamgiftsClient.ViewModels
{
    public class UserInfoViewModel : ViewModelBase
    {
        #region Reactive Properties
        [Reactive]
        public UserInfo UserInfo { get; set; }

        [Reactive]
        public Bitmap? Avatar { get; set; }
        #endregion

        private readonly ISiteManager _siteManager;

        public UserInfoViewModel(ISiteManager siteManager)
        {
            _siteManager = siteManager;
            UserInfo = _siteManager.UserInfo;

            LoadUserAvatarAsync();
        }

        private async void LoadUserAvatarAsync()
        {
            Avatar = await BitmapHelper.LoadFromURL(UserInfo.AvatarURL);
        }
    }
}
