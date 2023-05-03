using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SteamgiftsClient.Models
{
    public class Giveaway : ReactiveObject
    {
        public string ID { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public string Link { get; set; } = null!;
        public string Remaining { get; set; } = null!;
        public int Cost { get; set; }
        public double Level { get; set; }

        [Reactive]
        public bool Entered { get; set; } = false;
    }
}
