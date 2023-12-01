using SteamgiftsClient.Services.SiteManager;
using System.Collections.Generic;

namespace SteamgiftsClient.Models
{
    public class UserPreferences
    {
        public string PHPSESID { get; set; }

        public List<SearchCategory> EntryCategoriesOrder { get; set; }
    }
}
