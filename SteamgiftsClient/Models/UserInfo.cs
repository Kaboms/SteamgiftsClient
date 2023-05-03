using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SteamgiftsClient.Models
{
    public class UserInfo : ReactiveObject
    {
        public string Name { get; set; } = "...";
        public string URL { get; set; } = string.Empty;
        public string AvatarURL { get; set; } = string.Empty;
        public double Level { get; set; }

        [Reactive]
        public int Points { get; set; }

    }
}
