using HtmlAgilityPack;
using SteamgiftsClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamgiftsClient.Services.SiteManager
{
    internal class SiteManager : ISiteManager
    {
        public UserInfo UserInfo { get; set; } = new UserInfo();

        private static Uri _siteAddress = new Uri("https://www.steamgifts.com");

        private readonly CookieContainer _cookies;
        private string? _accessToken;

        public SiteManager()
        {
            _cookies = new CookieContainer();
        }

        public async Task<bool> LoginAsync(string phpsesid)
        {
            _cookies.Add(_siteAddress, new Cookie("PHPSESSID", phpsesid));

            var siteData = await GetResponseDataFromRequestAsync();
            if (siteData == null)
                return false;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(siteData);

            HtmlNodeCollection tokenNodes = document.DocumentNode.SelectNodes("//*[@name=\"xsrf_token\"]");
            if (tokenNodes != null)
                _accessToken = tokenNodes[0].GetAttributeValue("value", "");

            if (!string.IsNullOrEmpty(_accessToken))
            {
                await LoadUserInfoAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> SendAjaxRequestAsync(FormUrlEncodedContent ajaxContent, string giveawayLink)
        {
            using (var handler = new HttpClientHandler() { CookieContainer = _cookies })
            using (var client = new HttpClient(handler) { BaseAddress = _siteAddress })
            {
                HttpResponseMessage httpResponse;
                try
                {
                    string? httpResponseData = await GetResponseDataFromRequestAsync(giveawayLink, client);
                    if (httpResponseData == null || httpResponseData.Contains("Not Enough Points"))
                        return false;

                    // Try enter to giveaway
                    httpResponse = await client.PostAsync("ajax.php", ajaxContent);
                    httpResponse.EnsureSuccessStatusCode();

                    return true;
                }
                catch { }

            }
            return false;
        }

        public async Task<bool> EnterGiveawayAsync(Giveaway giveaway)
        {
            if (_accessToken == null)
                return false;

            FormUrlEncodedContent enterGaveawayContent = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["xsrf_token"] = _accessToken,
                ["do"] = "entry_insert",
                ["code"] = giveaway.ID
            });

            if (await SendAjaxRequestAsync(enterGaveawayContent, giveaway.Link))
            {
                giveaway.Entered = true;
                UserInfo.Points -= giveaway.Cost;

                return true;
            }
            return false;
        }

        public async Task<bool> RemoveEntryAsync(Giveaway giveaway)
        {
            if (_accessToken == null)
                return false;

            FormUrlEncodedContent enterGaveawayContent = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["xsrf_token"] = _accessToken,
                ["do"] = "entry_delete",
                ["code"] = giveaway.ID
            });

            if (await SendAjaxRequestAsync(enterGaveawayContent, giveaway.Link))
            {
                giveaway.Entered = false;
                UserInfo.Points += giveaway.Cost;
                return true;
            }
            return false;
        }

        public string GetTypeByCategory(SearchCategory category)
        {
            switch (category)
            {
                case SearchCategory.Wishlist: return "type=wishlist";
                case SearchCategory.Recommended: return "type=recommended";
                case SearchCategory.All: return "";
            }

            return "";
        }

        public async Task<List<Giveaway>> GetGiveawaysAsync(SearchCategory searchCategory, int page = 1)
        {
            List<Giveaway> result = new List<Giveaway>();

            string request = "giveaways/search?";
            string searchCategoryStr = GetTypeByCategory(searchCategory);
            request += searchCategoryStr;

            if (page > 1)
            {
                request += (searchCategoryStr == string.Empty ? "page=" : "&page=") + page;
            }

            var htmlData = await GetResponseDataFromRequestAsync(request);
            if (htmlData == null)
                return result;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlData);

            var giveawayNodes = document.DocumentNode.SelectNodes("//*[@class=\"giveaway__row-outer-wrap\"]").ToList();
            if (giveawayNodes == null)
                return result;

            // Filter nodes
            giveawayNodes.RemoveAll(node =>
            {
                //Remove pinned giveaways
                if (node.ParentNode.OuterHtml.Contains("pinned-giveaways__inner-wrap"))
                    return true;

                HtmlNode costNode = node.SelectSingleNode(".//*[@class=\"giveaway__heading__thin\"]");
                if (costNode == null)
                    return true;

                HtmlNode headingNode = node.SelectSingleNode(".//*[@class=\"giveaway__heading__name\"]");
                if (headingNode == null)
                    return true;

                HtmlNode imageNode = node.SelectSingleNode(".//*[@class=\"giveaway_image_thumbnail\"]");
                if (imageNode == null)
                    return true;

                return false;
            });

            foreach (var node in giveawayNodes)
            {
                HtmlNode innerNode = node.SelectSingleNode(".//*[@class=\"giveaway__row-inner-wrap is-faded\"]");
                bool entered = innerNode != null;

                HtmlNode costNode = node.SelectSingleNode(".//*[@class=\"giveaway__heading__thin\"]");
                int giveawaytCost = 0;
                Match match = Regex.Match(costNode.InnerText, @"\d+");
                if (match.Success)
                {
                    giveawaytCost = Convert.ToInt32(match.Value);
                }

                HtmlNode headingNode = node.SelectSingleNode(".//*[@class=\"giveaway__heading__name\"]");
                string giveawayURL = headingNode.GetAttributeValue("href", "");
                string gameName = headingNode.InnerText;

                HtmlNode imageNode = node.SelectSingleNode(".//*[@class=\"giveaway_image_thumbnail\"]");
                string imageURL = imageNode.GetAttributeValue("style", "");
                imageURL = imageURL.Substring(21);
                imageURL = imageURL.Remove(imageURL.Length - 2);

                double level = 1;
                HtmlNode levelNode = node.SelectSingleNode(".//*[@title=\"Contributor Level\"]");
                if (levelNode != null)
                {
                    match = Regex.Match(levelNode.InnerText, @"\d+");
                    if (match.Success)
                    {
                        level = Convert.ToInt32(match.Value);
                    }
                }

                string remaining = "";
                var timeNodes = node.SelectNodes(".//*[@data-timestamp]");
                if (timeNodes.Count > 0)
                {
                    remaining = timeNodes.First().InnerText;
                }

                result.Add(new Giveaway
                {
                    ID = giveawayURL.Substring(10, 5),
                    Name = gameName,
                    ImageURL = imageURL,
                    Link = _siteAddress + giveawayURL,
                    Cost = giveawaytCost,
                    Level = level,
                    Remaining = remaining,
                    Entered = entered
                });
            }

            return result;
        }

        public async Task LoadUserInfoAsync()
        {
            var responseString = await GetResponseDataFromRequestAsync();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            //Get user link and name
            HtmlNode profileLinkNode = document.DocumentNode.SelectSingleNode("//*[@class=\"nav__avatar-outer-wrap\"]");
            if (profileLinkNode != null)
            {
                UserInfo.URL = profileLinkNode.GetAttributeValue("href", "");
                UserInfo.Name = UserInfo.URL.Remove(0, UserInfo.URL.LastIndexOf("/") + 1);
            }

            //Get user avatar
            HtmlNode avatarNode = document.DocumentNode.SelectSingleNode("//*[@class=\"nav__avatar-inner-wrap\"]");
            if (avatarNode != null)
            {
                string avatarURL = avatarNode.GetAttributeValue("style", "");

                avatarURL = avatarURL.Substring(21);
                avatarURL = avatarURL.Remove(avatarURL.Length - 2);
                avatarURL = avatarURL.Replace("_medium", "_full");
                UserInfo.AvatarURL = avatarURL;
            }

            HtmlNode accountNode = document.DocumentNode.SelectSingleNode("//*[@class=\"nav__button nav__button--is-dropdown\" and @href=\"/account\"]");

            //Get user points
            HtmlNode pointsNode = accountNode.SelectSingleNode("//*[@class=\"nav__points\"]");
            if (pointsNode != null)
            {
                UserInfo.Points = Convert.ToInt32(pointsNode.InnerText);
            }

            //Get user level
            HtmlNode levelNode = accountNode.Elements("span").Last();
            if (levelNode != null)
            {
                string level = levelNode.GetAttributeValue("title", "1");
                UserInfo.Level = double.Parse(level, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Send request and get response data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<string?> GetResponseDataFromRequestAsync(string request = "", HttpClient? client = null)
        {
            using (var clientHandler = new HttpClientHandler() { CookieContainer = _cookies })
            {
                if (client == null)
                {
                    client = new HttpClient(clientHandler) { BaseAddress = _siteAddress };
                }

                HttpResponseMessage siteDataResponse;
                try
                {
                    siteDataResponse = await client.GetAsync(request);
                    siteDataResponse.EnsureSuccessStatusCode();
                    return await siteDataResponse.Content.ReadAsStringAsync();
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<bool> HasNextPage(int page)
        {
            string request = page > 1 ? "giveaways/search?page=" + page : string.Empty;

            var htmlData = await GetResponseDataFromRequestAsync(request);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlData);

            HtmlNode paginationNode = document.DocumentNode.SelectSingleNode("//*[@class=\"pagination__navigation\"]");
            return paginationNode.ChildNodes.Last().InnerHtml.ToLower().Contains("next");
        }
    }
}
