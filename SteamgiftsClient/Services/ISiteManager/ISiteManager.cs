using SteamgiftsClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamgiftsClient.Services.SiteManager
{
    public enum SearchCategory
    {
        Wishlist = 0,
        Recommended = 1,
        All = 2
    };

    /// <summary>
    /// Parse and manage www.steamgifts.com
    /// </summary>
    public interface ISiteManager
    {
        public UserInfo UserInfo { get; protected set; }

        /// <summary>
        /// To get access to the site we need access token to be initialized
        /// </summary>
        /// <returns>True if successful</returns>
        public Task<bool> LoginAsync(string phpsesid);

        /// <summary>
        /// Enter to giveaway.
        /// </summary>
        /// <param name="giveaway"></param>
        /// <returns>True on successful entry. False on error or not enougt points</returns>
        public Task<bool> EnterGiveawayAsync(Giveaway giveaway);

        /// <summary>
        /// Remove entry
        /// </summary>
        /// <param name="giveaway"></param>
        /// <returns>True on successful remove entry. False if giveaway already finished</returns>
        public Task<bool> RemoveEntryAsync(Giveaway giveaway);

        /// <summary>
        /// Get giveaways on page
        /// </summary>
        /// <param name="searchCategory"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public Task<List<Giveaway>> GetGiveawaysAsync(SearchCategory searchCategory, int page = 1);

        /// <summary>
        /// Async load user information using PHPSESID
        /// </summary>
        /// <returns></returns>
        public Task LoadUserInfoAsync();

        /// <summary>
        /// Check if a next page is exist
        /// </summary>
        /// <returns></returns>
        public Task<bool> HasNextPage(int page);
    }
}
