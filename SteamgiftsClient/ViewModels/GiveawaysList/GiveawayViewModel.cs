using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using SteamgiftsClient.Common;
using SteamgiftsClient.Common.Helpers;
using SteamgiftsClient.Models;
using SteamgiftsClient.Services.SiteManager;
using System.Reactive;
using System.Threading.Tasks;

namespace SteamgiftsClient.ViewModels
{
    public class GiveawayViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OpenGiveawayLink { get; set; }
        public ReactiveCommand<Unit, bool> EnterGiveaway { get; set; }
        public ReactiveCommand<Unit, bool> RemoveEntry { get; set; }

        [Reactive]
        public Giveaway GiveawayData { get; set; }

        public Task<Bitmap?> GameImage => BitmapHelper.LoadFromURL(GiveawayData.ImageURL);

        public GiveawayViewModel(Giveaway giveawayData)
        {
            GiveawayData = giveawayData;
            OpenGiveawayLink = ReactiveCommand.Create(() => CommonHelper.OpenUrl(GiveawayData.Link));

            EnterGiveaway = ReactiveCommand.CreateFromTask(t =>
                Locator.Current.GetRequiredService<ISiteManager>().EnterGiveawayAsync(GiveawayData));
            RemoveEntry = ReactiveCommand.CreateFromTask(t =>
                Locator.Current.GetRequiredService<ISiteManager>().RemoveEntryAsync(GiveawayData));
        }
    }
}
